namespace Nemonuri.Trees;

public class AncestorContext<TNode>
{
    public IChildrenProvider<TNode> Premise { get; }

    public TNode Root { get; }

    private readonly List<ChildAndIndex<TNode>> _childAndIndexSequence;

    public AncestorContext(IChildrenProvider<TNode> premise, TNode root)
    {
        Debug.Assert(premise is not null);
        Debug.Assert(root is not null);

        Premise = premise;
        Root = root;
        _childAndIndexSequence = new();
    }

    public IReadOnlyList<ChildAndIndex<TNode>>? ChildAndIndexSequence => _childAndIndexSequence;

    public bool TryPushChildAndIndex(TNode child, int index)
    {
        Debug.Assert(child is not null);

        if (_childAndIndexSequence.Count == 0)
        {
            return false;
        }

        (var maybeParent, var _) = _childAndIndexSequence[^1];

        if (maybeParent is null) { return false; }

        if (!Premise.IsParentAt(maybeParent, child, index))
        {
            return false;
        }

        _childAndIndexSequence.Add(new ChildAndIndex<TNode>(child, index));
        return true;
    }

    public bool TryPopChildAndIndex(out ChildAndIndex<TNode> childAndIndex)
    {
        if (_childAndIndexSequence.Count == 0)
        {
            childAndIndex = default;
            return false;
        }

        childAndIndex = _childAndIndexSequence[^1];
        _childAndIndexSequence.RemoveAt(_childAndIndexSequence.Count - 1);
        return true;
    }
}
