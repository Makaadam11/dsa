public class ReconciliationService
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

        string[] fundsLines = fundsInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string[] commitmentsLines = commitmentsInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string line in fundsLines)
        {
            string[] parts = line.Split('|');

            decimal.TryParse(parts[3], out decimal targetSize);

            funds.Add(new Fund
            {
                FundId = parts[0],
                FundName = parts[1],
                Strategy = parts[2],
                TargetSize = targetSize
            });
        }

        foreach (string line in commitmentsLines)
        {
            string[] parts = line.Split('|');

            decimal.TryParse(parts[2], out decimal amount);
            DateTime.TryParse(parts[3], out DateTime date);

            commitments.Add(new Commitment
            {
                InvestorName = parts[0],
                FundId = parts[1],
                Amount = amount,
                Date = date
            });
        }

        Dictionary<string, FundData> fundData = new();

        foreach (Fund f in funds)
        {
            decimal totalCommited = commitments
            .Where(c => c.FundId == f.FundId)
            .Select(c => c.Amount)
            .Sum();

            decimal targetSizePercent = (f.TargetSize - totalCommited) / f.TargetSize * 100m;

            int numOfInvestors = commitments
            .Where(c => c.FundId == f.FundId)
            .DistinctBy(c => c.InvestorName)
            .Count();

            fundData.Add(f.FundId, new FundData()
            {
                TotalCommited = totalCommited,
                TargetSizePercent = targetSizePercent,
                NumOfInvestors = numOfInvestors,
            });
        }


        // // jedno grupowanie zamiast Where per fund — O(n) zamiast O(funds × commitments)
        Dictionary<string, List<Commitment>> commitmentsByFund = commitments
            .GroupBy(c => c.FundId)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<string, FundData> fundData = new();

        foreach (Fund f in funds)
        {
            // fund bez commitmentów → pusta lista, nie wyjątek
            List<Commitment> fc = commitmentsByFund.TryGetValue(f.FundId, out var list) ? list : new();

            decimal totalCommitted = fc.Sum(c => c.Amount);

            fundData.Add(f.FundId, new FundData
            {
                TotalCommitted = totalCommitted,
                TargetSizePercent = f.TargetSize == 0 ? 0 : totalCommitted / f.TargetSize * 100m,
                NumOfInvestors = fc.Select(c => c.InvestorName).Distinct().Count()
            });
        }

        List<string> top3Funds = fundData
            .OrderByDescending(fd => fd.Value.TargetSizePercent)
            .ThenByDescending(fd => fd.Value.TotalCommitted)   // tie-breaker
            .Take(3)
            .Select(fd => fd.Key)
            .ToList();

        HashSet<string> existingFunds = funds.Select(f => f.FundId).ToHashSet();
        List<Commitment> orphans = commitments.Where(c => !existingFunds.Contains(c.FundId)).ToList();
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
