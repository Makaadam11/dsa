namespace Tasks;

using System.Collections.Generic;
using System.Linq;

public class PortfolioService
{
    private readonly List<Holding> _holdings;
    private decimal? _totalValueCached;   // mutable cache → plain nullable field, NOT readonly

    public PortfolioService(List<Holding> holdings)
    {
        _holdings = holdings;
    }

    public decimal GetTotalValue()
    {
        // Loop over an empty list sums to 0 anyway — only null needs a guard.
        decimal total = _holdings?.Sum(h => h.Quantity * h.PricePerUnit) ?? 0m;
        _totalValueCached = total;
        return total;
    }

    // Nullable return: with `required` properties `new Holding()` no longer compiles,
    // and a fake empty holding would be worse than an honest null anyway.
    public Holding? GetLargestHolding()
    {
        if (_holdings == null || _holdings.Count == 0)
            return null;

        return _holdings.MaxBy(h => h.Quantity * h.PricePerUnit);
    }

    public decimal GetWeight(Holding holding)
    {
        if (_holdings == null)
            return 0m;

        decimal totalValue = _totalValueCached ?? GetTotalValue();
        return totalValue != 0 ? holding.Quantity * holding.PricePerUnit / totalValue : 0m;
    }

    public List<Holding> TopNHoldingsByValue(int n)
    {
        if (_holdings == null)
            return new();

        return _holdings
            .OrderByDescending(h => h.Quantity * h.PricePerUnit)
            .Take(n)
            .ToList();
    }

    public List<Holding> GetHoldingsAboveWeight(decimal threshold)
    {
        if (_holdings == null)
            return new();

        return _holdings
            .Where(h => GetWeight(h) > threshold)
            .OrderByDescending(h => GetWeight(h))   // cheap: total is cached after first call
            .ToList();
    }
}

public class Holding
{
    public required string AssetName { get; set; }
    public required int Quantity { get; set; }
    public required decimal PricePerUnit { get; set; }
}
