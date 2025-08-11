namespace Nemonuri.Trees;

public static class WalkingTheory
{
    public static bool TryWalkAsRoot<TNode, TTarget>
    (
        IAggregatingPremise<IndexedPathWithNodePremise<TNode>, TTarget> aggregatingPremise,
        IChildrenProvider<TNode> nodePremise,
        TNode root,
        [NotNullWhen(true)] out TTarget? walkedValue
    )
    {
        Debug.Assert(aggregatingPremise is not null);
        Debug.Assert(nodePremise is not null);
        Debug.Assert(root is not null);

        IndexedPath<TNode> indexedPath = new(root);

        if (!TryWalkChildren(aggregatingPremise, nodePremise, indexedPath, out var childrenAggregated))
        { goto Fail; }

        if
        (
            !aggregatingPremise.TryAggregate
            (
                aggregatingPremise.DefaultSeed, childrenAggregated,
                new IndexedPathWithNodePremise<TNode>(indexedPath, nodePremise),
                out walkedValue
            )
        )
        { goto Fail; }

        return true;

    Fail:
        walkedValue = default;
        return false;
    }

    public static bool TryWalkChildren<TNode, TTarget>
    (
        IAggregatingPremise<IndexedPathWithNodePremise<TNode>, TTarget> aggregatingPremise,
        IChildrenProvider<TNode> nodePremise,
        IndexedPath<TNode> indexedPath,
        [NotNullWhen(true)] out TTarget? childrenAggregated
    )
    {
        Debug.Assert(aggregatingPremise is not null);
        Debug.Assert(nodePremise is not null);

        if (!indexedPath.TryGetLastNode(out var node))
        { goto Fail; }

        childrenAggregated = aggregatingPremise.DefaultSeed;

        int childIndex = 0;
        foreach (var child in nodePremise.GetChildren(node))
        {
            var childIndexedPath = indexedPath.Push(child, childIndex);

            if
            (
                !TryWalkChildren
                (
                    aggregatingPremise, nodePremise, childIndexedPath,
                    out var grandChildrenAggregated
                )
            )
            { goto Fail; }


            if
            (
                !aggregatingPremise.TryAggregate
                (
                    childrenAggregated, grandChildrenAggregated,
                    new(childIndexedPath, nodePremise),
                    out var nextChildrenAggregated
                )
            )
            { goto Fail; }

            childrenAggregated = nextChildrenAggregated;
            childIndex++;
        }

        return childrenAggregated is not null;

    Fail:
        childrenAggregated = default;
        return false;
    }
}