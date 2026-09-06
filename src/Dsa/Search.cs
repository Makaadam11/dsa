namespace Dsa;

public static class Search
{
    public static int BinarySearch<T>(IReadOnlyList<T> items, T target) where T : IComparable<T>
    {
        var left = 0;
        var right = items.Count - 1;

        while (left <= right)
        {
            var mid = left + ((right - left) / 2);
            var cmp = items[mid].CompareTo(target);

            if (cmp == 0)
                return mid;

            if (cmp < 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }

    public static int LowerBound<T>(IReadOnlyList<T> items, T target) where T : IComparable<T>
    {
        var left = 0;
        var right = items.Count;

        while (left < right)
        {
            var mid = left + ((right - left) / 2);

            if (items[mid].CompareTo(target) < 0)
                left = mid + 1;
            else
                right = mid;
        }

        return left;
    }
}
