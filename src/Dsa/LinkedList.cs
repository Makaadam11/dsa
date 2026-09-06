namespace Dsa;

public sealed class ListNode<T>(T value, ListNode<T>? next = null)
{
    public T Value { get; set; } = value;
    public ListNode<T>? Next { get; set; } = next;
}

public sealed class SinglyLinkedList<T>
{
    public ListNode<T>? Head { get; private set; }
    public int Count { get; private set; }

    public void AddFirst(T value)
    {
        Head = new ListNode<T>(value, Head);
        Count++;
    }

    public void AddLast(T value)
    {
        var node = new ListNode<T>(value);
        Count++;

        if (Head is null)
        {
            Head = node;
            return;
        }

        var current = Head;
        while (current.Next is not null)
            current = current.Next;

        current.Next = node;
    }

    public void Reverse()
    {
        ListNode<T>? prev = null;
        var current = Head;

        while (current is not null)
        {
            var next = current.Next;
            current.Next = prev;
            prev = current;
            current = next;
        }

        Head = prev;
    }

    public IEnumerable<T> AsEnumerable()
    {
        for (var current = Head; current is not null; current = current.Next)
            yield return current.Value;
    }
}
