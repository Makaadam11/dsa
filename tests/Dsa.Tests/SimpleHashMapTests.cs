using Dsa;

namespace Dsa.Tests;

public class SimpleHashMapTests
{
    [Fact]
    public void Put_SameKeyTwice_UpdatesValueWithoutGrowingCount()
    {
        var map = new SimpleHashMap<string, int>();
        map.Put("apple", 1);
        map.Put("banana", 2);
        map.Put("apple", 99);

        Assert.Equal(99, map.Get("apple"));
        Assert.Equal(2, map.Get("banana"));
        Assert.Equal(2, map.Count);
    }

    [Fact]
    public void Remove_DeletesKey()
    {
        var map = new SimpleHashMap<string, int>();
        map.Put("apple", 1);
        map.Remove("apple");

        Assert.False(map.Contains("apple"));
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void Get_MissingKey_Throws()
    {
        var map = new SimpleHashMap<string, int>();
        Assert.Throws<KeyNotFoundException>(() => map.Get("xyz"));
    }

    [Fact]
    public void Remove_MissingKey_Throws()
    {
        var map = new SimpleHashMap<string, int>();
        Assert.Throws<KeyNotFoundException>(() => map.Remove("xyz"));
    }

    [Fact]
    public void CollidingKeys_StayIndependent()
    {
        var map = new SimpleHashMap<int, string>(capacity: 1);
        for (var i = 0; i < 50; i++)
            map.Put(i, $"v{i}");

        Assert.Equal(50, map.Count);
        Assert.Equal("v37", map.Get(37));
    }

    [Fact]
    public void Indexer_ReadsAndWrites()
    {
        var map = new SimpleHashMap<string, int> { ["k"] = 5 };
        Assert.Equal(5, map["k"]);
    }
}
