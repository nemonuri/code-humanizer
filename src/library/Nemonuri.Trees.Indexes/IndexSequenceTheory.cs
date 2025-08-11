namespace Nemonuri.Trees.Indexes;

public static class IndexSequenceTheory
{
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
}