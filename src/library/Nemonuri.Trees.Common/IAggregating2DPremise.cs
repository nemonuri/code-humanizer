
namespace Nemonuri.Trees;

public interface IAggregating2DPremise<TSource, TTarget>
{
    TTarget DefaultSeed { get; }
    
    bool TryAggregate
    (
        TTarget siblingsSeed,
        TTarget childrenSeed,
        TSource source,
        [NotNullWhen(true)] out TTarget? aggregated
    );
}