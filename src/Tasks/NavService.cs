namespace Tasks;

public class NavService
{
    static string navsInput = """
    F001|2024-03-31|100000000
    F001|2024-06-30|105000000
    F001|2024-12-31|108045000
    F001|2024-09-30|102900000
    F002|2024-03-31|200000000
    F002|2024-06-30|190000000
    F002|2024-09-30|n/a
    F002|2024-12-31|209000000
    F003|2024-03-31|50000000
    F003|2024-06-30|55000000

    F003|2024-09-30|48000000
    F003|2024-06-30|60000000
    F001|2024-02-30|101000000
    F003|2024-12-31
    F004|2024-06-30|75000000
    """;

    public static void Main()
    {
        // ---------- 1. PARSE (twoje, bez zmian poza i+1 i TrimEntries) ----------
        List<Snapshot> snapshotsRaw = new();
        List<string> errors = new();

        string[] lines = navsInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split('|');

            if (parts.Length != 3) { errors.Add($"Line {i + 1}: not enough fields."); continue; }
            if (!DateTime.TryParse(parts[1], out DateTime date)) { errors.Add($"Line {i + 1}: bad Date."); continue; }
            if (!decimal.TryParse(parts[2], out decimal navValue)) { errors.Add($"Line {i + 1}: bad NavValue."); continue; }

            snapshotsRaw.Add(new Snapshot { FundId = parts[0], Date = date, NavValue = navValue });
        }

        // ---------- 2. DEDUP — last wins ----------
        // GroupBy zachowuje kolejność z pliku, więc Last() = ostatnia linia dla tej pary (FundId, Date)
        List<Snapshot> snapshots = snapshotsRaw
            .GroupBy(s => new { s.FundId, s.Date })
            .Select(g => g.Last())
            .ToList();

        // ---------- 3. RETURNS + GAPS — per fund, sort, porównaj sąsiadów ----------
        List<Returns> fundReturns = new();
        List<string> gaps = new();

        foreach (var fundGroup in snapshots.GroupBy(s => s.FundId))
        {
            List<Snapshot> sorted = fundGroup.OrderBy(s => s.Date).ToList();

            for (int i = 1; i < sorted.Count; i++)
            {
                Snapshot prev = sorted[i - 1];
                Snapshot cur = sorted[i];

                int monthsApart = (cur.Date.Year - prev.Date.Year) * 12 + (cur.Date.Month - prev.Date.Month);
                if (monthsApart > 3)
                    gaps.Add($"{fundGroup.Key}: missing quarter between {prev.Date:yyyy-MM-dd} and {cur.Date:yyyy-MM-dd}");

                fundReturns.Add(new Returns
                {
                    FundId = fundGroup.Key,
                    Date = cur.Date,
                    NavValue = cur.NavValue,
                    ReturnsPercent = (cur.NavValue / prev.NavValue - 1) * 100m
                });
            }
        }

        Returns? bestQuarter = fundReturns.MaxBy(fr => fr.ReturnsPercent);
        Returns? worstQuarter = fundReturns.MinBy(fr => fr.ReturnsPercent);

        // ---------- 4. OUTPUT ----------
        Console.WriteLine("=== PARSE ERRORS (expected 3: n/a, 2024-02-30, 2 fields) ===");
        foreach (string e in errors) Console.WriteLine($"  {e}");
        Console.WriteLine($"Raw: {snapshotsRaw.Count} (expected 12) | After dedup: {snapshots.Count} (expected 11)\n");

        Console.WriteLine("=== RETURNS (expected F001: +5, -2, +5 | F002: -5, +10 | F003: +20, -20 | F004: none) ===");
        foreach (Returns r in fundReturns)
            Console.WriteLine($"  {r.FundId}  {r.Date:yyyy-MM-dd}  nav={r.NavValue:N0}  return={r.ReturnsPercent:+0.0;-0.0}%");

        Console.WriteLine("\n=== GAPS (expected 1: F002 between 2024-06-30 and 2024-12-31) ===");
        foreach (string g in gaps) Console.WriteLine($"  {g}");

        Console.WriteLine($"\nBEST  (expected F003 2024-06-30 +20.0%): {bestQuarter?.FundId} {bestQuarter?.Date:yyyy-MM-dd} {bestQuarter?.ReturnsPercent:+0.0;-0.0}%");
        Console.WriteLine($"WORST (expected F003 2024-09-30 -20.0%): {worstQuarter?.FundId} {worstQuarter?.Date:yyyy-MM-dd} {worstQuarter?.ReturnsPercent:+0.0;-0.0}%");
    }
}

public class Snapshot
{
    public string FundId { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal NavValue { get; set; }
}

public class Returns
{
    public string FundId { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal NavValue { get; set; }
    public decimal ReturnsPercent { get; set; }
}