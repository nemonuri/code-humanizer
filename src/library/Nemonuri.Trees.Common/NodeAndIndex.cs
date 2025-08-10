namespace Nemonuri.Trees;

public readonly struct NodeWithIndex<TNode>
{
    public TNode? Node { get; }
    public int? Index { get; }

    public NodeWithIndex(TNode? node, int? index)
    {
        Node = node;
        Index = index;
    }

    public void Deconstruct(out TNode? node, out int? index)
    {
        node = Node;
        index = Index;
    }
}