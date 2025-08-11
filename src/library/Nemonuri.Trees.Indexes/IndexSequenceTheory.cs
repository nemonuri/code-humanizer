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
        Guard.IsFalse(inserted.IsReferencingRoot);

        // Check `inserted` is shorter or equal than `source`
        if (!(inserted.Count <= source.Count)) { return source; }

        int updatingIndex = inserted.Count - 1;

        // Check `inserted` and `source` are structurally equal until `updatingIndex`
        for (int i = 0; i < updatingIndex; i++)
        {
            if (!(inserted[i] == source[i])) { return source; }
        }

        // Check `inserted[updatingIndex]` is less or equal than `source[updatingIndex]`
        if (!(inserted[updatingIndex] <= source[updatingIndex]))
        { return source; }

        return source.SetItem(updatingIndex, source[updatingIndex] + 1);
    }
    

}