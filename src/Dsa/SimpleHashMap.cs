namespace Dsa;

public sealed class SimpleHashMap<TKey, TValue> where TKey : notnull
{
    private readonly List<KeyValuePair<TKey, TValue>>[] _buckets;

    public SimpleHashMap(int capacity = 16)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);

        _buckets = new List<KeyValuePair<TKey, TValue>>[capacity];
        for (var i = 0; i < capacity; i++)
            _buckets[i] = [];
    }

    public int Count { get; private set; }

    public TValue this[TKey key]
    {
        get => Get(key);
        set => Put(key, value);
    }

    public void Put(TKey key, TValue value)
    {
        var bucket = BucketFor(key);

        for (var i = 0; i < bucket.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(bucket[i].Key, key))
            {
                bucket[i] = new KeyValuePair<TKey, TValue>(key, value);
                return;
            }
        }

        bucket.Add(new KeyValuePair<TKey, TValue>(key, value));
        Count++;
    }

    public TValue Get(TKey key)
    {
        if (TryGet(key, out var value))
            return value;

        throw new KeyNotFoundException($"{key}");
    }

    public bool TryGet(TKey key, out TValue value)
    {
        foreach (var entry in BucketFor(key))
        {
            if (EqualityComparer<TKey>.Default.Equals(entry.Key, key))
            {
                value = entry.Value;
                return true;
            }
        }

        value = default!;
        return false;
    }

    public void Remove(TKey key)
    {
        var bucket = BucketFor(key);

        for (var i = 0; i < bucket.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(bucket[i].Key, key))
            {
                bucket.RemoveAt(i);
                Count--;
                return;
            }
        }

        throw new KeyNotFoundException($"{key}");
    }

    public bool Contains(TKey key) => TryGet(key, out _);

    private List<KeyValuePair<TKey, TValue>> BucketFor(TKey key)
    {
        var index = (int)((uint)key.GetHashCode() % (uint)_buckets.Length);
        return _buckets[index];
    }
}
