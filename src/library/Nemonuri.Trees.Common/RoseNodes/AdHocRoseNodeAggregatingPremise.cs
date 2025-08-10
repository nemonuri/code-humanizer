
namespace Nemonuri.Trees.RoseNodes;

public class AdHocRoseNodeAggregatingPremise<T, TTarget>
    : IAggregatingPremise<WalkingNodeInfo<RoseNode<T>>, TTarget>
{
    private readonly AdHocAggregatingPremise<WalkingNodeInfo<RoseNode<T>>, TTarget> _internalPremise;

    private AdHocRoseNodeAggregatingPremise(AdHocAggregatingPremise<WalkingNodeInfo<RoseNode<T>>, TTarget> internalPremise)
    {
        Debug.Assert(internalPremise is not null);

        _internalPremise = internalPremise;
    }

    public AdHocRoseNodeAggregatingPremise
    (
        Func<TTarget> defaultSeedProvider,
        TryAggregator<WalkingNodeInfo<RoseNode<T>>, TTarget> tryAggregator
    )
    : this(new(defaultSeedProvider, tryAggregator))
    { }

    public AdHocRoseNodeAggregatingPremise
    (
        Func<TTarget> defaultSeedProvider,
        OptionalAggregator<WalkingNodeInfo<RoseNode<T>>, TTarget> optionalAggregator
    )
    : this(new(defaultSeedProvider, optionalAggregator))
    { }


    public TTarget DefaultSeed => _internalPremise.DefaultSeed;

    public bool TryAggregate(TTarget seed, WalkingNodeInfo<RoseNode<T>> source, [NotNullWhen(true)] out TTarget? aggregated) =>
        _internalPremise.TryAggregate(seed, source, out aggregated);
}
