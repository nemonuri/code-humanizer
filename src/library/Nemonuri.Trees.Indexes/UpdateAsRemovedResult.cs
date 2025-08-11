namespace Nemonuri.Trees.Indexes;

public readonly struct UpdateAsRemovedResult
{
    internal UpdateAsRemovedResult(bool boundInRemovedSubtree, IndexSequence indexSequence)
    {
        Debug.Assert(indexSequence is not null);
        BoundInRemovedSubtree = boundInRemovedSubtree;
        IndexSequence = indexSequence;
    }

    public bool BoundInRemovedSubtree { get; }
    public IndexSequence IndexSequence { get; }
}