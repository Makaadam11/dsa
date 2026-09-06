namespace Tasks;

using System;
using System.Collections.Generic;
using System.Linq;

public class TransactionService
{
    private readonly List<Transaction> _transactions;

    public TransactionService(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    // Pipeline rule: filter BEFORE aggregate — Sum ends the chain.
    public decimal GetDistributionsSum()
    {
        return _transactions
            .Where(t => t.Type == "Distribution")
            .Sum(t => t.Amount);
    }

    // Net cashflow = distributions − capital calls (money returned minus money called).
    // One pass per group: conditional sign inside Sum instead of two scans.
    public Dictionary<string, decimal> GetNetCashflowPerFund()
    {
        return _transactions
            .GroupBy(t => t.FundName)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Type == "Distribution" ? t.Amount : -t.Amount));
    }

    // Group → aggregate → sort desc → take → project the key. Returns fund NAMES.
    public List<string> GetTopNFundsByCapitalCalls(int n)
    {
        return _transactions
            .Where(t => t.Type == "CapitalCall")
            .GroupBy(t => t.FundName)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .Take(n)
            .Select(g => g.Key)
            .ToList();
    }

    public List<Transaction> GetTransactionsForAYear(int year)
    {
        return _transactions
            .Where(t => t.Date.Year == year)
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public List<string> GetFundsWithPositiveCashflow()
    {
        return _transactions
            .GroupBy(t => t.FundName)
            .Where(g => g.Sum(t => t.Type == "Distribution" ? t.Amount : -t.Amount) > 0)
            .Select(g => g.Key)
            .ToList();
    }

    public static void Run()
    {
        // Expected values computed BY HAND first (the June rule) — printed next to actuals.

        // var transactions = new List<Transaction>
        // {
        //     new() { FundName = "Fund A", Type = "CapitalCall",  Amount = 500m, Date = new DateTime(2025, 3, 10) },
        //     new() { FundName = "Fund A", Type = "Distribution", Amount = 700m, Date = new DateTime(2025, 9, 1) },
        //     new() { FundName = "Fund B", Type = "CapitalCall",  Amount = 900m, Date = new DateTime(2024, 5, 20) },
        //     new() { FundName = "Fund B", Type = "Distribution", Amount = 300m, Date = new DateTime(2025, 1, 15) },
        //     new() { FundName = "Fund C", Type = "CapitalCall",  Amount = 200m, Date = new DateTime(2025, 6, 5) },
        // };

        // var service = new TransactionService(transactions);

        // Console.WriteLine($"Distributions sum (expected 1000): {service.GetDistributionsSum()}");

        // Console.WriteLine("Net cashflow per fund (expected A: 200, B: -600, C: -200):");
        // foreach (var (fund, net) in service.GetNetCashflowPerFund())
        //     Console.WriteLine($"  {fund}: {net}");

        // Console.WriteLine($"Top 2 funds by capital calls (expected B, A): " +
        //     string.Join(", ", service.GetTopNFundsByCapitalCalls(2)));

        // Console.WriteLine($"Transactions in 2025 (expected 4): {service.GetTransactionsForAYear(2025).Count}");

        // Console.WriteLine($"Funds with positive cashflow (expected A): " +
        //     string.Join(", ", service.GetFundsWithPositiveCashflow()));
    }
}

public class Transaction
{
    public required string FundName { get; set; }
    public required string Type { get; set; }  // "CapitalCall" or "Distribution"
    public required decimal Amount { get; set; }
    public required DateTime Date { get; set; }
}
