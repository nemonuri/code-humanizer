namespace Nemonuri.Trees;

public static class AggregatingTheory
{
    public static bool TryAggregateAll<TSource, TTarget>
    (
        this IAggregatingPremise<TSource, TTarget> premise,
        TTarget seed,
        IEnumerable<TSource> sources,
        [NotNullWhen(true)] out TTarget? aggregated
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(seed is not null);
        Debug.Assert(sources is not null);

        var currentSeed = seed;
        foreach (var source in sources)
        {
            if (!premise.TryAggregate(currentSeed, source, out var nextSeed)) { goto Fail; }
            currentSeed = nextSeed;
        }

        aggregated = currentSeed;
        return true;

    Fail:
        aggregated = default;
        return false;
    }

    public static bool TryAggregateAll<TSource, TTarget>
    (
        this IAggregatingPremise<TSource, TTarget> premise,
        IEnumerable<TSource> sources,
        [NotNullWhen(true)] out TTarget? aggregated
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(sources is not null);

        return premise.TryAggregateAll(premise.DefaultSeed, sources, out aggregated);
    }

    public static bool TrySelect<TSource, TTarget>
    (
        this IAggregatingPremise<TSource, TTarget> premise,
        TSource source,
        [NotNullWhen(true)] out TTarget? selected
    )
    {
        return premise.TryAggregate
        (
            premise.DefaultSeed,
            source,
            out selected
        );
    }

}