namespace Nemonuri.Trees.Indexes;

public static class Int32ReadOnlyListCompareTheory
{
    public static bool AreEqual
    (
        IReadOnlyList<int>? x,
        IReadOnlyList<int>? y
    )
    {
        if (x is null || y is null) { return false; }
        if (!(x.Count == y.Count)) { return false; }
        for (int i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i]) { return false; }
        }
        return true;
    }

    public static int CalculateHashCode(IReadOnlyList<int>? source)
    {
        if (source is null) { return 0; }
        HashCode hashCode = new();
        foreach (var item in source)
        {
            hashCode.Add(item);
        }
        return hashCode.ToHashCode();
    }

    public static int CompareFromDownLeftToTopRight
    (
        IReadOnlyList<int>? x,
        IReadOnlyList<int>? y
    )
    {
        switch (x, y)
        {
            case (null, null):
                return 0;
            case ({ }, null):
                return 1;
            case (null, { }):
                return -1;
            case var (x1, y1) when x1.Count < y1.Count:
                return 1;
            case var (x1, y1) when x1.Count > y1.Count:
                return -1;
            default:
                break;
        }

        for (int i = 0; i < x.Count; i++)
        {
            int elementCompareResult = x[i].CompareTo(y[i]);
            if (elementCompareResult != 0) { return elementCompareResult; }
        }

        return 0;
    }
}