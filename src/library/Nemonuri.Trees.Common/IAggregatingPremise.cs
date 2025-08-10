
namespace Nemonuri.Trees;

public interface IAggregatingPremise<TSource, TTarget>
{
    TTarget DefaultSeed { get; }

    bool TryAggregate
    (
        TTarget seed,
        TSource source,
        [NotNullWhen(true)] out TTarget? aggregated
    );
}