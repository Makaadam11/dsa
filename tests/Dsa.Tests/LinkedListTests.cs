using Dsa;

namespace Dsa.Tests;

public class LinkedListTests
{
    [Fact]
    public void Reverse_ReversesOrder()
    {
        var list = new SinglyLinkedList<int>();
        foreach (var v in new[] { 1, 2, 3, 4 })
            list.AddLast(v);

        list.Reverse();

        Assert.Equal([4, 3, 2, 1], list.AsEnumerable());
    }

    [Fact]
    public void Reverse_EmptyList_StaysEmpty()
    {
        var list = new SinglyLinkedList<int>();
        list.Reverse();

        Assert.Null(list.Head);
        Assert.Empty(list.AsEnumerable());
    }

    [Fact]
    public void AddFirst_PrependsAndCounts()
    {
        var list = new SinglyLinkedList<int>();
        list.AddFirst(2);
        list.AddFirst(1);

        Assert.Equal([1, 2], list.AsEnumerable());
        Assert.Equal(2, list.Count);
    }
}
