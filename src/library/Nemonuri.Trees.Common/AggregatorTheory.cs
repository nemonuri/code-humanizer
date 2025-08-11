namespace Nemonuri.Trees;

public static class AggregatorTheory
{
    public static
    TryAggregator<TSource, TTarget>
    ToTryAggregator<TSource, TTarget>
    (
        this OptionalAggregator<TSource, TTarget> optionalAggregator
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
            var result = optionalAggregator(siblingsSeed, childrenSeed, source);
            aggregated = result.Item2 ? result.Item1 : default;
            return aggregated is not null;
        }
    }
}

public delegate bool TryAggregator<TSource, TTarget>
(
    TTarget siblingsSeed,
    TTarget childrenSeed,
    TSource source,
    [NotNullWhen(true)] out TTarget? aggregated
);

public delegate (TTarget?, bool) OptionalAggregator<TSource, TTarget>
(
    TTarget siblingsSeed,
    TTarget childrenSeed,
    TSource source
);