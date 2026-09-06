using Dsa;

namespace Dsa.Tests;

public class SearchTests
{
    [Theory]
    [InlineData(5, 2)]
    [InlineData(4, -1)]
    [InlineData(1, 0)]
    [InlineData(9, 4)]
    public void BinarySearch_FindsIndexOrMinusOne(int target, int expected)
    {
        int[] arr = [1, 3, 5, 7, 9];
        Assert.Equal(expected, Search.BinarySearch(arr, target));
    }

    [Theory]
    [InlineData(5, 2)]
    [InlineData(4, 2)]
    [InlineData(0, 0)]
    [InlineData(10, 5)]
    public void LowerBound_ReturnsInsertionPoint(int target, int expected)
    {
        int[] arr = [1, 3, 5, 7, 9];
        Assert.Equal(expected, Search.LowerBound(arr, target));
    }

    [Fact]
    public void LowerBound_WithDuplicates_ReturnsFirstOccurrence()
    {
        int[] arr = [1, 3, 3, 3, 5, 7];
        Assert.Equal(1, Search.LowerBound(arr, 3));
    }

    [Fact]
    public void BinarySearch_EmptyArray_ReturnsMinusOne()
    {
        Assert.Equal(-1, Search.BinarySearch(Array.Empty<int>(), 1));
    }
}
