using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public static class RoseNodeAggregatingPremiseTestTheory
{
    public static AdHocRoseNodeAggregatingPremise<T, bool>
    CreateForallPremiseUsingOptionalAggregator<T>
    (
        RoseNodePremise<T> roseNodePremise,
        Func<T?, bool> predicate
    )
    {
        Assert.NotNull(roseNodePremise);
        Assert.NotNull(predicate);

        AdHocRoseNodeAggregatingPremise<T, bool> aggregatingPremise = new
        (
            defaultSeedProvider: () => true,
            optionalAggregator: (childrenSeed, siblingsSeed, source) =>
            {
                if (!(childrenSeed && siblingsSeed)) { return (false, true); }
                if (!source.IndexedPath.TryGetLastNode(out var node)) { return (false, false); }
                return (predicate(node.Value), true);
            }
        );

        return aggregatingPremise;
    }
}