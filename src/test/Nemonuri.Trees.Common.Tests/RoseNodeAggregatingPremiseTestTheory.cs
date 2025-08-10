using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public static class RoseNodeAggregatingPremiseTestTheory
{
    public static AdHocRoseNodeAggregatingPremise<T, bool>
    CreateForallPremiseUsingTryAggregator<T>
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
            tryAggregator: (bool seed, WalkingNodeInfo<RoseNode<T>> source, out bool aggregated) =>
            {
                if (seed == false)
                {
                    aggregated = false;
                    return true;
                }

                var child = source.ChildAndIndex.Child;
                if (child is null) { goto Fail; }

                T? value = roseNodePremise.GetValue(child);
                aggregated = predicate(value);

                return true;

            Fail:
                aggregated = false;
                return false;
            }
        );

        return aggregatingPremise;
    }

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
            optionalAggregator: (seed, source) =>
                seed == false ? (false, true) :
                (source.ChildAndIndex.Child is not { } child) ? (false, false) :
                (predicate(roseNodePremise.GetValue(child)), true)
        );

        return aggregatingPremise;
    }
}