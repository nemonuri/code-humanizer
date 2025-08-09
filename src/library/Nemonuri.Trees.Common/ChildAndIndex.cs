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

    public ChildAndIndex(TNode? child): this(child, -1)
    { }

    public void Deconstruct(out TNode? child, out int index)
    {
        child = Child;
        index = Index;
    }

    public bool IsDefault => (Child is null) && (Index == 0);

    public ChildAndIndexState State
    {
        get
        {
            if (Child is null || Index < -1) { return ChildAndIndexState.Invalid; }
            return (Index == -1) ? ChildAndIndexState.ChildOnly : ChildAndIndexState.ChildWithIndex;
        }
    }

    [Conditional("DEBUG")]
    [MemberNotNull(nameof(Child))]
    internal void AssertStateIsChildOnly()
    {
        Debug.Assert(State is ChildAndIndexState.ChildOnly);
        Debug.Assert(Child is not null);
    }

    [Conditional("DEBUG")]
    [MemberNotNull(nameof(Child))]
    internal void AssertStateIsChildWithIndex()
    {
        Debug.Assert(State is ChildAndIndexState.ChildWithIndex);
        Debug.Assert(Child is not null);
    }
}

public enum ChildAndIndexState
{
    Invalid = 0,
    ChildOnly = 1,
    ChildWithIndex = 2
}