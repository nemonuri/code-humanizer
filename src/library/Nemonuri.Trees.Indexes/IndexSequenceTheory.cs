namespace Nemonuri.Trees.Indexes;

public static class IndexSequenceTheory
{
    public static IndexSequence CreateIndexSequence(ReadOnlySpan<int> items)
    {
        ImmutableList<int> internalList = [.. items];
        return new IndexSequence(internalList);
    }

    public static IndexSequence ToIndexSequence<TNode>
    (
        this IndexedPath<TNode> indexedPath
    )
    {
        Guard.IsTrue(indexedPath.HasRoot);

        ImmutableList<int>.Builder builder = ImmutableList<int>.Empty.ToBuilder();
        for (int i = 1; i < indexedPath.Count; i++)
        {
            (var _, var index) = indexedPath[i];
            Guard.IsNotNull(index, $"{nameof(indexedPath)}[{i}]");
            builder.Add(index.Value);
        }

        return new IndexSequence(builder.ToImmutable());
    }

    public static IndexSequence UpdateAsInserted
    (
        this IndexSequence source,
        IndexSequence inserted
    )
    {
        Debug.Assert(source is not null);
        Debug.Assert(inserted is not null);
        Guard.IsFalse(inserted.IsReferencingRoot);

        // Check `inserted` is shorter or equal than `source` (:= p0)
        if (!(inserted.Count <= source.Count)) { return source; }

        int updatingIndex = inserted.Count - 1;

        // Check `inserted` and `source` are structurally equal until `updatingIndex` (:= p1)
        for (int i = 0; i < updatingIndex; i++)
        {
            if (!(inserted[i] == source[i])) { return source; }
        }

        // Check `inserted[updatingIndex]` is less or equal than `source[updatingIndex]` (:= p2)
        if (!(inserted[updatingIndex] <= source[updatingIndex]))
        { return source; }

        return source.SetItem(updatingIndex, source[updatingIndex] + 1);
    }

    public static bool TryGetBoundInSubtree
    (
        this IndexSequence source,
        IndexSequence subtreeRoot,
        [NotNullWhen(true)] out IndexSequence? bound
    )
    {
        Debug.Assert(source is not null);
        Debug.Assert(subtreeRoot is not null);

        // Check `subtreeRoot` is referencing root (:= p0)
        if (subtreeRoot.IsReferencingRoot)
        {
            bound = source;
            return true;
        }

        // Check `subtreeRoot` is shorter or equal than `source` (:= p1)
        if (!(subtreeRoot.Count <= source.Count)) { goto Fail; }

        // Check `source` start with `subtreeRoot` (:= p2)
        for (int i = 0; i < subtreeRoot.Count; i++)
        {
            if (!(subtreeRoot[i] == source[i])) { goto Fail; }
        }

        bound = source[subtreeRoot.Count..];
        return true;

    Fail:
        bound = default;
        return false;
    }

}