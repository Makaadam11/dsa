namespace Tasks;

using System;
using System.Collections.Generic;
using System.Linq;

// Tuesday pairing-simulation output, cleaned up.
// NOTE: renamed from TransactionService — the original duplicated the class name
// (and the Transaction model) already defined in TransactionService.cs, which
// breaks compilation. Uses the shared Transaction model instead.
public class SimulationService
{
    private readonly List<Transaction> _transactions;

    public SimulationService(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    public List<string> GetTopNFundsByType(int n, string type)
    {
        return _transactions
            .Where(t => t.Type == type)
            .GroupBy(t => t.FundName)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .Take(n)
            .Select(g => g.Key)
            .ToList();
    }

    // Optional date range: null bound = open side.
    // "x == null || condition" reads cleaner than the ternary-to-true trick.
    public List<string> GetFundsWithPositiveCashflowForDate(DateTime? dateFrom, DateTime? dateTo)
    {
        return _transactions
            .Where(t => (dateFrom == null || t.Date >= dateFrom)
                     && (dateTo == null || t.Date <= dateTo))
            .GroupBy(t => t.FundName)
            .Where(g => g.Sum(t => t.Type == "Distribution" ? t.Amount : -t.Amount) > 0)
            .Select(g => g.Key)
            .ToList();
    }

    public static void Run()
    {
        // // Simulation service — same data, date-filtered variant
        // var sim = new SimulationService(transactions);
        // Console.WriteLine($"Positive cashflow in 2025 only (expected A): " +
        //     string.Join(", ", sim.GetFundsWithPositiveCashflowForDate(new DateTime(2025, 1, 1), null)));
    }
}
