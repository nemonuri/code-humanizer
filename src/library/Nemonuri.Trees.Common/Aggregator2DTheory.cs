namespace Nemonuri.Trees;

public static class Aggregator2DTheory
{
    public static
    TryAggregator2D<TSource, TTarget>
    ToTryAggregator<TSource, TTarget>
    (
        this OptionalAggregator2D<TSource, TTarget> optionalAggregator
    )
    {
        Debug.Assert(optionalAggregator is not null);

        return TryAggregatorImpl;

        bool TryAggregatorImpl
        (
            TTarget siblingsSeed,
            TTarget childrenSeed,
            TSource source,
            [NotNullWhen(true)] out TTarget? aggregated
        )
        {
            var result = optionalAggregator(siblingsSeed, siblingsSeed, source);
            aggregated = result.Item2 ? result.Item1 : default;
            return aggregated is not null;
        }
    }
}

public delegate bool TryAggregator2D<TSource, TTarget>
(
    TTarget siblingsSeed,
    TTarget childrenSeed,
    TSource source,
    [NotNullWhen(true)] out TTarget? aggregated
);

public delegate (TTarget?, bool) OptionalAggregator2D<TSource, TTarget>
(
    TTarget siblingsSeed,
    TTarget childrenSeed,
    TSource source
);