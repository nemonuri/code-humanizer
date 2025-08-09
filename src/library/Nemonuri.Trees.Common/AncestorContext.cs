namespace Nemonuri.Trees;

public class AncestorContext<TNode>
{
    public IChildrenProvider<TNode> Premise { get; }

    public TNode? Root { get; private set; }

    private readonly List<ChildAndIndex<TNode>> _childAndIndexSequence;

    public AncestorContext(IChildrenProvider<TNode> premise, TNode? root)
    {
        Debug.Assert(premise is not null);

        Premise = premise;
        Root = root;
        _childAndIndexSequence = new();
    }

    public AncestorContext(IChildrenProvider<TNode> premise) : this(premise, default)
    { }

    public IReadOnlyList<ChildAndIndex<TNode>>? ChildAndIndexSequence => _childAndIndexSequence;

    public bool TryPushChildAndIndex(TNode child, int index) =>
        TryPushChildAndIndex(new(child, index));

    public bool TryPushChildAndIndex(ChildAndIndex<TNode> childAndIndex) =>
        TryPushChildAndIndexCore(childAndIndex, checkOnly: false);

    public bool CanPushChildAndIndex(ChildAndIndex<TNode> childAndIndex) =>
        TryPushChildAndIndexCore(childAndIndex, checkOnly: true);

    private bool TryPushChildAndIndexCore
    (
        ChildAndIndex<TNode> childAndIndex,
        bool checkOnly
    )
    {
        var childAndIndexState = childAndIndex.State;

        if (childAndIndexState == ChildAndIndexState.ChildOnly)
        {
            childAndIndex.AssertStateIsChildOnly();

            if (Root is not null) { return false; }

            if (!checkOnly)
            {
                Root = childAndIndex.Child;
            }

            return true;
        }
        else if (childAndIndexState == ChildAndIndexState.ChildWithIndex)
        {
            childAndIndex.AssertStateIsChildWithIndex();

            TNode? maybeParent;
            if (_childAndIndexSequence.Count == 0)
            {
                maybeParent = Root;
            }
            else
            {
                (maybeParent, var _) = _childAndIndexSequence[^1];
            }

            if (maybeParent is null) { return false; }

            (var child, var index) = childAndIndex;

            Debug.Assert(child is not null);

            if (!Premise.IsParentAt(maybeParent, child, index))
            {
                return false;
            }

            if (!checkOnly)
            {
                _childAndIndexSequence.Add(new ChildAndIndex<TNode>(child, index));
            }
            return true;
        }
        else /*if (childAndIndexState == ChildAndIndexState.Invalid)*/
        {
            return false;
        }
    }

    public bool TryPopChildAndIndex(out ChildAndIndex<TNode> childAndIndex) =>
        TryPopChildAndIndexCore(peekOnly: false, out childAndIndex);

    public bool TryPeekChildAndIndex(out ChildAndIndex<TNode> childAndIndex) =>
        TryPopChildAndIndexCore(peekOnly: true, out childAndIndex);
    
    private bool TryPopChildAndIndexCore
    (
        bool peekOnly,
        out ChildAndIndex<TNode> childAndIndex
    )
    {
        if (_childAndIndexSequence.Count == 0)
        {
            if (Root is null)
            {
                childAndIndex = default;
                return false;
            }
            else
            {
                childAndIndex = new ChildAndIndex<TNode>(Root);
                if (!peekOnly)
                {
                    Root = default;
                }
                return true;
            }
        }
        else
        {
            childAndIndex = _childAndIndexSequence[^1];
            if (!peekOnly)
            {
                _childAndIndexSequence.RemoveAt(_childAndIndexSequence.Count - 1);
            }
            return true;
        }
    }
}

//public delegate T Aggregator<T>(T aggregated, T aggregating);