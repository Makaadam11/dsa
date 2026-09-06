namespace Tasks;

public class FundDataService
{
    private readonly IFundRepository _repo;

    public FundDataService(IFundRepository repo)
    {
        _repo = repo;
    }

    // async + Task<T> + await — the three always travel together.
    public async Task<decimal> GetFundNavAsync(string fundId)
    {
        List<FundHolding> holdings = await _repo.GetHoldingsAsync(fundId);
        return holdings.Sum(h => h.Value);
    }
}

public interface IFundRepository
{
    Task<List<FundHolding>> GetHoldingsAsync(string fundId);
}

public class FundHolding
{
    public decimal Value { get; set; }
}
