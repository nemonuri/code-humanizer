

namespace Nemonuri.Trees;

public class AdHocAggregating2DPremise<TSource, TTarget> : IAggregating2DPremise<TSource, TTarget>
{
    public AdHocAggregating2DPremise(Func<TTarget> defaultSeedProvider, TryAggregator2D<TSource, TTarget> tryAggregator)
    {
        Debug.Assert(defaultSeedProvider is not null);
        Debug.Assert(tryAggregator is not null);

        DefaultSeedProvider = defaultSeedProvider;
        TryAggregator = tryAggregator;
    }

    public AdHocAggregating2DPremise
    (
        Func<TTarget> defaultSeedProvider,
        OptionalAggregator2D<TSource, TTarget> optionalAggregator
    )
    : this(defaultSeedProvider, optionalAggregator.ToTryAggregator())
    { }

    public Func<TTarget> DefaultSeedProvider { get; }

    public TryAggregator2D<TSource, TTarget> TryAggregator { get; }

    public TTarget DefaultSeed => DefaultSeedProvider.Invoke();

    public bool TryAggregate(TTarget siblingsSeed, TTarget childrenSeed, TSource source, [NotNullWhen(true)] out TTarget? aggregated) =>
        TryAggregator.Invoke(siblingsSeed, childrenSeed, source, out aggregated);
}