namespace Nemonuri.Trees;

public interface IChildrenProvider<TNode>
{ 
    IEnumerable<TNode> GetChildren(TNode source);
}

public interface IRoseNodePremise<TValue, TNode> : IChildrenProvider<TNode>
{
    TValue GetValue(TNode source);
}
