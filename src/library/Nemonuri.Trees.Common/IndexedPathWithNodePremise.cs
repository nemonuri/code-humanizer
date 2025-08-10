namespace Nemonuri.Trees;

public readonly struct IndexedPathWithNodePremise<TNode>
{
    public IndexedPath<TNode> IndexedPath { get; }
    public IChildrenProvider<TNode>? NodePremise { get; }

    public IndexedPathWithNodePremise(IndexedPath<TNode> indexedPath, IChildrenProvider<TNode>? nodePremise)
    {
        IndexedPath = indexedPath;
        NodePremise = nodePremise;
    }
}