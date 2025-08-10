
namespace Nemonuri.Trees;

public class AdHocAggregatingPremise<TSource, TTarget> : IAggregatingPremise<TSource, TTarget>
{
    public Func<TTarget> DefaultSeedProvider { get; }

    public TryAggregator<TSource, TTarget> TryAggregator { get; }

    public AdHocAggregatingPremise(Func<TTarget> defaultSeedProvider, TryAggregator<TSource, TTarget> tryAggregator)
    {
        Debug.Assert(defaultSeedProvider is not null);
        Debug.Assert(tryAggregator is not null);

        DefaultSeedProvider = defaultSeedProvider;
        TryAggregator = tryAggregator;
    }

    public TTarget DefaultSeed => DefaultSeedProvider.Invoke();

    public bool TryAggregate(TTarget seed, TSource source, [NotNullWhen(true)] out TTarget? aggregated) =>
        TryAggregator.Invoke(seed, source, out aggregated);
}