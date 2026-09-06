namespace Live;

public class ReconciliationServicePractice
{
    static string fundsInput = """
        F001|Apex Growth III|Buyout|500000000
        F002|Meridian Credit Fund|Private Debt|300000000
        F003|Northgate Ventures II|Venture Capital|150000000
        F006|Atlas RE Partners|Real Estate|250000000
        """;

    static string commitmentsInput = """
        Alpha Pension|F001|25000000|2024-03-15
        Beta Insurance|F001|40000000|2024-04-02
        Alpha Pension|F001|15000000|2024-06-20
        Gamma Endowment|F002|60000000|2024-01-10
        Delta SWF|F002|90000000|2024-02-28
        Epsilon Family Office|F003|30000000|2024-05-05
        Zeta Capital|F003|45000000|2024-07-19
        Eta Foundation|F006|20000000|2024-08-01
        Theta Pension|F999|50000000|2024-03-30
        """;

    public static void Main()
    {
        List<Fund> funds = new();
        List<Commitment> commitments = new();

        string[] linesFunds = fundsInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string[] linesCommitments = commitmentsInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string line in linesFunds)
        {
            string[] parts = line.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!decimal.TryParse(parts[3], out decimal tSize)) { continue; }

            funds.Add(new Fund
            {
                FundId = parts[0],
                FundName = parts[1],
                Strategy = parts[2],
                TargetSize = tSize
            });
        }

        foreach (string line in linesCommitments)
        {
            string[] parts = line.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!decimal.TryParse(parts[2], out decimal amo)) { continue; }
            if (!DateTime.TryParse(parts[3], out DateTime dt)) { continue; }

            commitments.Add(new Commitment
            {
                InvestorName = parts[0],
                FundId = parts[1],
                Amount = amo,
                Date = dt
            });
        }

        Dictionary<string, FundData> fundData = new();

        Dictionary<string, List<Commitment>> perFundCommitments = commitments
        .GroupBy(c => c.FundId)
        .ToDictionary(g => g.Key,
                        g => g.ToList());

        foreach (Fund f in funds)
        {
            List<Commitment>? comm = !perFundCommitments.TryGetValue(f.FundId, out List<Commitment> list) ? new() : list;

            decimal totalCommited = comm.Select(c => c.Amount).Sum();

            fundData.Add(f.FundId, new FundData
            {
                TotalCommited = totalCommited,
                TargetSizePercent = totalCommited / f.TargetSize * 100,
                NumOfInvestors = comm.Select(c => c.InvestorName).Distinct().Count()
            });
        }

        List<string> top3Funds = fundData
        .OrderByDescending(g => g.Value.TargetSizePercent)
        .Take(3)
        .Select(g => g.Key)
        .ToList();

        HashSet<string> fundsActive = funds.Select(f => f.FundId).ToHashSet();

        List<Commitment> orphans = commitments.Where(c => !fundsActive.Contains(c.FundId)).ToList();

        foreach ((string fId, FundData fd) in fundData)
        {
            Console.WriteLine($"Total commited {fId} {fd.TotalCommited}");
            Console.WriteLine($"TargetSizePercent {fId} {fd.TargetSizePercent}");
            Console.WriteLine($"NumOfInvestors {fId} {fd.NumOfInvestors}");
        }

        Console.WriteLine($"{}");
        Console.WriteLine($"{}");
    }
}
public class Fund
{
    public string FundId { get; set; }
    public string FundName { get; set; }
    public string Strategy { get; set; }
    public decimal TargetSize  { get; set; }
}
public class Commitment
{
    public string InvestorName { get; set; }
    public string FundId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
public class FundData
{
    public decimal TotalCommited  { get; set; }
    public decimal TargetSizePercent  { get; set; }
    public int NumOfInvestors  { get; set; }
}