namespace Nemonuri.Trees;

public static class WalkingTheory
{
    [Conditional("DEBUG")]
    internal static void AssertTrySelectRequirements<TNode>
    (
        AncestorContext<TNode> ancestorContext,
        ChildAndIndex<TNode> childAndIndex
    )
    {
        Debug.Assert(ancestorContext is not null);
        Debug.Assert(ancestorContext.CanPushChildAndIndex(childAndIndex));
    }

    public static bool TryWalkAsNode<TNode, TTarget>
    (
        this IAggregatingPremise<WalkingNodeInfo<TNode>, TTarget> aggregatingPremise,
        AncestorContext<TNode> ancestorContext,
        ChildAndIndex<TNode> childAndIndex,
        [NotNullWhen(true)] out TTarget? walkedValue
    )
    {
        Debug.Assert(aggregatingPremise is not null);
        Debug.Assert(ancestorContext is not null);

        if (!ancestorContext.TryPushChildAndIndex(childAndIndex))
        { goto Fail; }

        if (!aggregatingPremise.TryWalkChildren(ancestorContext, out var walkChildrenValue))
        { goto Fail; }

        if (!ancestorContext.TryPopChildAndIndex(out _))
        { goto Fail; }

        if
        (
            !aggregatingPremise.TryAggregate
            (
                walkChildrenValue,
                new WalkingNodeInfo<TNode>
                {
                    AncestorContext = ancestorContext,
                    ChildAndIndex = childAndIndex
                },
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
        this IAggregatingPremise<WalkingNodeInfo<TNode>, TTarget> aggregatingPremise,
        AncestorContext<TNode> ancestorContext,
        [NotNullWhen(true)] out TTarget? walkedValue
    )
    {
        Debug.Assert(aggregatingPremise is not null);
        Debug.Assert(ancestorContext is not null);

        if (!ancestorContext.TryPeekChildAndIndex(out var childAndIndex))
        {
            goto Fail;
        }

        (var parent, var _) = childAndIndex;

        if (parent is null)
        {
            goto Fail;
        }

        if
        (
            !aggregatingPremise.TryAggregateAll
            (
                ancestorContext.Premise.GetChildren(parent)
                    .Select
                    (
                        (n, i) => new WalkingNodeInfo<TNode>()
                        {
                            AncestorContext = ancestorContext,
                            ChildAndIndex = new ChildAndIndex<TNode>(n, i)
                        }
                    ),
                out walkedValue
            )
        )
        { goto Fail; }

        return true;

    Fail:
        walkedValue = default;
        return false;
    }

    public static bool TryWalkAsRoot<TNode, TTarget>
    (
        this IAggregatingPremise<WalkingNodeInfo<TNode>, TTarget> aggregatingPremise,
        IChildrenProvider<TNode> premise,
        TNode root,
        [NotNullWhen(true)] out TTarget? walkedValue
    )
    {
        Debug.Assert(aggregatingPremise is not null);
        Debug.Assert(premise is not null);
        Debug.Assert(root is not null);

        AncestorContext<TNode> ancestorContext = new(premise);

        return
            aggregatingPremise.TryWalkAsNode
            (
                ancestorContext,
                new ChildAndIndex<TNode>(root),
                out walkedValue
            );
    }
}