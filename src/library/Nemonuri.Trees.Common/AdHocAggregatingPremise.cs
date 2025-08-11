

namespace Nemonuri.Trees;

public class AdHocAggregatingPremise<TSource, TTarget> : IAggregatingPremise<TSource, TTarget>
{
    public AdHocAggregatingPremise(Func<TTarget> defaultSeedProvider, TryAggregator<TSource, TTarget> tryAggregator)
    {
        Debug.Assert(defaultSeedProvider is not null);
        Debug.Assert(tryAggregator is not null);

        DefaultSeedProvider = defaultSeedProvider;
        TryAggregator = tryAggregator;
    }

    public AdHocAggregatingPremise
    (
        Func<TTarget> defaultSeedProvider,
        OptionalAggregator<TSource, TTarget> optionalAggregator
    )
    : this(defaultSeedProvider, optionalAggregator.ToTryAggregator())
    { }

    public Func<TTarget> DefaultSeedProvider { get; }

    public TryAggregator<TSource, TTarget> TryAggregator { get; }

    public TTarget DefaultSeed => DefaultSeedProvider.Invoke();

    public bool TryAggregate(TTarget siblingsSeed, TTarget childrenSeed, TSource source, [NotNullWhen(true)] out TTarget? aggregated) =>
        TryAggregator.Invoke(siblingsSeed, childrenSeed, source, out aggregated);
}