namespace Nemonuri.Trees;

public static class ParentTheory
{
    public static bool IsParent<TNode>
    (
        this IChildrenProvider<TNode> premise,
        TNode maybeParent,
        TNode maybeChild,
        IEqualityComparer<TNode>? equalityComparer = null
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(maybeParent is not null);
        Debug.Assert(maybeChild is not null);

        var children = premise.GetChildren(maybeParent);
        return
            children.Contains(maybeChild, equalityComparer);
    }

    public static int GetChildrenCount<TNode>
    (
        this IChildrenProvider<TNode> premise,
        TNode source
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(source is not null);

        var children = premise.GetChildren(source);

        return children.Count();
    }

    public static TNode GetChildAt<TNode>
    (
        this IChildrenProvider<TNode> premise,
        TNode source,
        int index
    )
    {
        int childCount = premise.GetChildrenCount(source);
        Guard.IsInRange(index, 0, childCount);

        return
            premise.GetChildren(source) switch
            {
                IReadOnlyList<TNode> v1 => v1[index],
                var v2 => v2.ElementAt(index)
            };
    }

    public static bool TryGetChildAt<TNode>
    (
        this IChildrenProvider<TNode> premise,
        TNode source,
        int index,
        [NotNullWhen(true)] out TNode? child
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(source is not null);

        var children = premise.GetChildren(source);

        if (children is IReadOnlyList<TNode> v1)
        {
            if (index < v1.Count)
            {
                child = v1[index];
            }
            else
            {
                child = default;
            }
        }
        else
        {
            child = children.ElementAtOrDefault(index);
        }

        return child is not null;
    }

    public static bool IsParentAt<TNode>
    (
        this IChildrenProvider<TNode> premise,
        TNode maybeParent,
        TNode maybeChild,
        int maybeIndex,
        IEqualityComparer<TNode>? equalityComparer = null
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(maybeParent is not null);
        Debug.Assert(maybeChild is not null);

        if (!premise.TryGetChildAt(maybeParent, maybeIndex, out var child))
        {
            return false;
        }

        if (equalityComparer is null)
        {
            return maybeChild.Equals(child);
        }
        else
        {
            return equalityComparer.Equals(maybeChild, child);
        }
    }

    public static bool TryGetDecendentOrSelfAt<TNode>
    (
        this IChildrenProvider<TNode> premise,
        TNode source,
        IEnumerable<int> indexSequence,
        [NotNullWhen(true)] out TNode? decendentOrSelf
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(source is not null);
        Debug.Assert(indexSequence is not null);

        TNode currentNode = source;
        foreach (var index in indexSequence)
        {
            if (!premise.TryGetChildAt(currentNode, index, out var nextNode))
            {
                decendentOrSelf = default;
                return false;
            }

            currentNode = nextNode;
        }

        decendentOrSelf = currentNode;
        return true;
    }
}
