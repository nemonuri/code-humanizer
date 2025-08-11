using System.Collections;

namespace Nemonuri.Trees;

public readonly struct IndexedPath<TNode> : IReadOnlyList<NodeWithIndex<TNode>>
{
    private readonly ImmutableList<NodeWithIndex<TNode>> _internalList;

    private IndexedPath(ImmutableList<NodeWithIndex<TNode>> internalList)
    {
        _internalList = internalList;
    }

    public IndexedPath(TNode root)
    {
        Guard.IsNotNull(root);

        _internalList = [new NodeWithIndex<TNode>(root, default)];
    }

    public IndexedPath<TNode> Push(TNode node, int index)
    {
        Guard.IsTrue(HasRoot);
        Guard.IsNotNull(node);
        Guard.IsGreaterThanOrEqualTo(index, 0);

        return new(_internalList.Add(new(node, index)));
    }

    public bool CanPop => _internalList.Count > 1;

    public bool HasRoot => _internalList.Count > 0;

    public IndexedPath<TNode> Pop()
    {
        Guard.IsGreaterThan(Count, 1);

        return new(_internalList.RemoveAt(_internalList.Count - 1));
    }

    public NodeWithIndex<TNode> Peek()
    {
        Guard.IsGreaterThan(Count, 0);

        return _internalList[^1];
    }

    public NodeWithIndex<TNode> this[int index] => _internalList[index];

    public int Count => _internalList.Count;

    public IEnumerator<NodeWithIndex<TNode>> GetEnumerator() => _internalList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool TryGetLastNode([NotNullWhen(true)] out TNode? lastNode)
    {
        if (!HasRoot) { goto Fail; }

        if (Peek().Node is not { } node)
        { goto Fail; }

        lastNode = node;
        return true;

    Fail:
        lastNode = default;
        return false;
    }
}
