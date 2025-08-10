namespace Nemonuri.Trees;

public interface IRoseNodePremise<TValue, TNode> : IChildrenProvider<TNode>
{
    TValue? GetValue(TNode source);
}
