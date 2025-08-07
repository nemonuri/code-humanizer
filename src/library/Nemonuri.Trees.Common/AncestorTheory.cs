namespace Nemonuri.Trees;

public static class AncestorTheory
{
    public static bool IsAncestorSequence<TNode>
    (
        this IChildrenProvider<TNode> premise,
        IReadOnlyList<TNode> source
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(source is not null);

        var count = source.Count;
        if (count <= 1) { return true; }

        for (int i = count - 2; i >= 0; i--)
        {
            var maybeParent = source[i];
            Debug.Assert(maybeParent is not null, $"{nameof(source)}[{i}] should not null");

            var maybeChild = source[i + 1];
            Debug.Assert(maybeChild is not null, $"{nameof(source)}[{i + 1}] should not null");

            bool sourceIsParent = premise.IsParent(maybeParent, maybeChild);
            if (!sourceIsParent) { return false; }
        }

        return true;
    }

    public static bool IsCompatibleIndexSequence<TNode>
    (
        this IChildrenProvider<TNode> premise,
        IReadOnlyList<int> indexSequence,
        IReadOnlyList<TNode> ancestorSequence,
        IEqualityComparer<TNode>? equalityComparer = null
    )
    {
        Debug.Assert(premise is not null);
        Debug.Assert(indexSequence is not null);
        Debug.Assert(ancestorSequence is not null);

        var ancestorSequenceCount = ancestorSequence.Count;
        if (ancestorSequenceCount <= 1)
        {
            return indexSequence.Count == 0;
        }

        if (ancestorSequenceCount > 1)
        {
            if ((indexSequence.Count - 1) != ancestorSequenceCount)
            {
                return false;
            }
        }

        for (int i = ancestorSequenceCount - 2; i >= 0; i--)
        {
            var maybeParent = ancestorSequence[i];
            Debug.Assert(maybeParent is not null, $"{nameof(ancestorSequence)}[{i}] should not null");

            var maybeChild = ancestorSequence[i + 1];
            Debug.Assert(maybeChild is not null, $"{nameof(ancestorSequence)}[{i + 1}] should not null");

            int maybeIndex = indexSequence[i];

            if (!premise.IsParentAt(maybeParent, maybeChild, maybeIndex, equalityComparer))
            {
                return false;
            }
        }

        return true;
    }
}