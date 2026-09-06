namespace Tasks.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Tasks;
using Xunit;
using Moq;

public class FundDataServiceTest
{
    [Fact]
    public async Task GetFundNavAsync_SumsHoldings()
    {
        // Arrange — mock the DEPENDENCY (IFundRepository), never the service under test
        var mockRepo = new Mock<IFundRepository>();
        mockRepo
            .Setup(r => r.GetHoldingsAsync("fund-1"))   // lambda; name must match the interface exactly
            .ReturnsAsync(new List<FundHolding>
            {
                new() { Value = 100.0M },
                new() { Value = 250.0M },
            });
        var service = new FundDataService(mockRepo.Object);

        // Act
        decimal result = await service.GetFundNavAsync("fund-1");

        // Assert — expected value FIRST
        Assert.Equal(350m, result);
    }

    [Fact]
    public async Task GetFundNavAsync_EmptyHoldings_ReturnsZero()
    {
        var mockRepo = new Mock<IFundRepository>();
        mockRepo
            .Setup(r => r.GetHoldingsAsync("empty-fund"))
            .ReturnsAsync(new List<FundHolding>());
        var service = new FundDataService(mockRepo.Object);

        decimal result = await service.GetFundNavAsync("empty-fund");

        Assert.Equal(0m, result);
    }
}
