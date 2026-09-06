namespace Live.Runner;

using System.Globalization;

// TASK A — Commitment Reconciliation (reference "100%" version)
// Pairing-realistic: brute force in Main, but with every robustness point covered:
//   1. Short/malformed lines guarded (parts.Length) BEFORE indexing
//   2. TryParse for numbers, TryParseExact for dates ("yyyy-MM-dd" — capital MM = months!)
//   3. Errors COLLECTED with line content + reason, not silently skipped
//   4. Division guard (TargetSize <= 0)
//   5. Dictionary built once, loop over FUNDS, TryGetValue per fund (O(n), zero-commitment funds kept)
//   6. Distinct investors counted case-safely
//   7. Expected values printed next to actuals — self-verification
// Parsing pulled into two small static methods: that IS the answer to
// "how would you test this?" — the parser is the most important test target.

public class ReconciliationService
{
    // Dirty on purpose: F004 has "abc" as TargetSize, F005 has only 3 fields.
    static string fundsInput = """
        F001|Apex Growth III|Buyout|500000000
        F002|Meridian Credit Fund|Private Debt|300000000
        F003|Northgate Ventures II|Venture Capital|150000000
        F004|Broken Fund|Buyout|abc
        F005|Short Line|Growth
        F006|Atlas RE Partners|Real Estate|250000000
        """;

    // Dirty on purpose: "xyz" amount, month 13 date, one 3-field line.
    static string commitmentsInput = """
        Alpha Pension|F001|25000000|2024-03-15
        Beta Insurance|F001|40000000|2024-04-02
        Alpha Pension|F001|15000000|2024-06-20
        Gamma Endowment|F002|60000000|2024-01-10
        Delta SWF|F002|90000000|2024-02-28
        Iota Trust|F002|xyz|2024-05-01
        Epsilon Family Office|F003|30000000|2024-05-05
        Zeta Capital|F003|45000000|2024-07-19
        Kappa Fund|F001|10000000|2024-13-45
        Eta Foundation|F006|20000000|2024-08-01
        Lambda Partners|F003|5000000
        Theta Pension|F999|50000000|2024-03-30
        """;

    public static void Main()
    {
        List<string> errors = new();

        List<Fund> funds = ParseFunds(fundsInput, errors);
        List<Commitment> commitments = ParseCommitments(commitmentsInput, errors);

        // --- Aggregate per fund ---------------------------------------------
        // Group commitments ONCE, then loop over funds (the parent entity)
        // and look up children in O(1). Funds with zero commitments stay in.
        Dictionary<string, List<Commitment>> perFundCommitments = commitments
            .GroupBy(c => c.FundId)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<string, FundData> fundData = new();

        foreach (Fund f in funds)
        {
            List<Commitment> comm = perFundCommitments.TryGetValue(f.FundId, out var list)
                ? list
                : new();

            decimal totalCommitted = comm.Sum(c => c.Amount);

            // Guard the division — a fund with TargetSize 0 must not crash the run.
            decimal percent = f.TargetSize > 0
                ? totalCommitted / f.TargetSize * 100
                : 0;

            fundData.Add(f.FundId, new FundData
            {
                TotalCommitted = totalCommitted,
                TargetSizePercent = percent,
                NumOfInvestors = comm.Select(c => c.InvestorName).Distinct().Count()
            });
        }

        // --- Top 3 by % funded (tie-break: alphabetical FundId, deterministic) ---
        List<string> top3Funds = fundData
            .OrderByDescending(kv => kv.Value.TargetSizePercent)
            .ThenBy(kv => kv.Key)
            .Take(3)
            .Select(kv => kv.Key)
            .ToList();

        // --- Orphans: commitments pointing at unknown funds ------------------
        HashSet<string> knownFunds = funds.Select(f => f.FundId).ToHashSet();
        List<Commitment> orphans = commitments
            .Where(c => !knownFunds.Contains(c.FundId))
            .ToList();

        // --- Output with EXPECTED values printed alongside -------------------
        // Expected: F001 80,000,000 / 16% / 2   F002 150,000,000 / 50% / 2
        //           F003 75,000,000 / 50% / 2   F006 20,000,000 / 8%  / 1
        foreach ((string fId, FundData fd) in fundData)
        {
            Console.WriteLine(
                $"{fId}: committed {fd.TotalCommitted:N0}, " +
                $"{fd.TargetSizePercent:0.##}% of target, " +
                $"{fd.NumOfInvestors} investor(s)");
        }

        Console.WriteLine($"Top 3: {string.Join(", ", top3Funds)} (expected F002, F003, F001)");

        Console.WriteLine($"Orphans ({orphans.Count}, expected 1):");
        orphans.ForEach(o => Console.WriteLine($"  {o.InvestorName} -> {o.FundId} ({o.Amount:N0})"));

        Console.WriteLine($"Skipped lines ({errors.Count}, expected 5):");
        errors.ForEach(e => Console.WriteLine($"  {e}"));
    }

    // --- Parsers: small, static, testable --------------------------------
    static List<Fund> ParseFunds(string input, List<string> errors)
    {
        List<Fund> funds = new();
        string[] lines = input.Split('\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            // Guard BEFORE indexing — a short line must not throw.
            if (parts.Length < 4)
            {
                errors.Add($"Fund line has {parts.Length} fields, expected 4: '{line}'");
                continue;
            }

            if (!decimal.TryParse(parts[3], out decimal targetSize))
            {
                errors.Add($"Fund line has invalid TargetSize '{parts[3]}': '{line}'");
                continue;
            }

            funds.Add(new Fund
            {
                FundId = parts[0],
                FundName = parts[1],
                Strategy = parts[2],
                TargetSize = targetSize
            });
        }
        return funds;
    }

    static List<Commitment> ParseCommitments(string input, List<string> errors)
    {
        List<Commitment> commitments = new();
        string[] lines = input.Split('\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            if (parts.Length < 4)
            {
                errors.Add($"Commitment line has {parts.Length} fields, expected 4: '{line}'");
                continue;
            }

            if (!decimal.TryParse(parts[2], out decimal amount))
            {
                errors.Add($"Commitment line has invalid Amount '{parts[2]}': '{line}'");
                continue;
            }

            // TryParseExact + "yyyy-MM-dd": capital MM is months (mm = minutes),
            // InvariantCulture so it behaves the same on any machine.
            // Rejects month 13 outright.
            if (!DateTime.TryParseExact(parts[3], "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                errors.Add($"Commitment line has invalid Date '{parts[3]}': '{line}'");
                continue;
            }

            commitments.Add(new Commitment
            {
                InvestorName = parts[0],
                FundId = parts[1],
                Amount = amount,
                Date = date
            });
        }
        return commitments;
    }
}

// decimal for money (exact base-10, no float drift), DateTime for dates.
// required + init would be nicer, but plain setters are fine at pairing pace.
public class Fund
{
    public string FundId { get; set; } = "";
    public string FundName { get; set; } = "";
    public string Strategy { get; set; } = "";
    public decimal TargetSize { get; set; }
}

public class Commitment
{
    public string InvestorName { get; set; } = "";
    public string FundId { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

public class FundData
{
    public decimal TotalCommitted { get; set; }
    public decimal TargetSizePercent { get; set; }
    public int NumOfInvestors { get; set; }   // int — it's a count, not money
}

/* EXPECTED OUTPUT ------------------------------------------------------
F001: committed 80,000,000, 16% of target, 2 investor(s)
F002: committed 150,000,000, 50% of target, 2 investor(s)
F003: committed 75,000,000, 50% of target, 2 investor(s)
F006: committed 20,000,000, 8% of target, 1 investor(s)
Top 3: F002, F003, F001 (expected F002, F003, F001)
Orphans (1, expected 1):
  Theta Pension -> F999 (50,000,000)
Skipped lines (5, expected 5):
  Fund line has invalid TargetSize 'abc': 'F004|Broken Fund|Buyout|abc'
  Fund line has 3 fields, expected 4: 'F005|Short Line|Growth'
  Commitment line has invalid Amount 'xyz': 'Iota Trust|F002|xyz|2024-05-01'
  Commitment line has invalid Date '2024-13-45': 'Kappa Fund|F001|10000000|2024-13-45'
  Commitment line has 3 fields, expected 4: 'Lambda Partners|F003|5000000'
----------------------------------------------------------------------- */