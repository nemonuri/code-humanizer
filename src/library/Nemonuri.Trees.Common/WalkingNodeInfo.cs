namespace Nemonuri.Trees;

public class WalkingNodeInfo<TNode>
{
    public required AncestorContext<TNode> AncestorContext;

    public ChildAndIndex<TNode> ChildAndIndex;
}
