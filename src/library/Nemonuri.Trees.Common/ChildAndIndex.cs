namespace Nemonuri.Trees;

public readonly struct ChildAndIndex<TNode>
{
    public TNode? Child { get; }
    public int Index { get; }

    public ChildAndIndex(TNode? child, int index)
    {
        Child = child;
        Index = index;
    }

    public void Deconstruct(out TNode? child, out int index)
    {
        child = Child;
        index = Index;
    }
}