
namespace Nemonuri.Trees.RoseNodes;

public class AdHocRoseNodeAggregating2DPremise<T, TTarget>
    : IAggregating2DPremise<IndexedPathWithNodePremise<RoseNode<T>>, TTarget>
{
    private readonly AdHocAggregating2DPremise<IndexedPathWithNodePremise<RoseNode<T>>, TTarget> _internalPremise;

    private AdHocRoseNodeAggregating2DPremise(AdHocAggregating2DPremise<IndexedPathWithNodePremise<RoseNode<T>>, TTarget> internalPremise)
    {
        Debug.Assert(internalPremise is not null);

        _internalPremise = internalPremise;
    }

    public AdHocRoseNodeAggregating2DPremise
    (
        Func<TTarget> defaultSeedProvider,
        TryAggregator2D<IndexedPathWithNodePremise<RoseNode<T>>, TTarget> tryAggregator
    )
    : this(new(defaultSeedProvider, tryAggregator))
    { }

    public AdHocRoseNodeAggregating2DPremise
    (
        Func<TTarget> defaultSeedProvider,
        OptionalAggregator2D<IndexedPathWithNodePremise<RoseNode<T>>, TTarget> optionalAggregator
    )
    : this(new(defaultSeedProvider, optionalAggregator))
    { }

    public TTarget DefaultSeed => _internalPremise.DefaultSeed;

    public bool TryAggregate
    (
        TTarget siblingsSeed, TTarget childrenSeed, IndexedPathWithNodePremise<RoseNode<T>> source,
        [NotNullWhen(true)] out TTarget? aggregated
    ) =>
    _internalPremise.TryAggregate(siblingsSeed, childrenSeed, source, out aggregated);
}