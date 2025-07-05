namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public interface IChildBoundNodeProvider<TBoundNode> : IChildBoundNodeProvider
    where TBoundNode : IBoundNode
{
    new IReadOnlyList<TBoundNode> ChildBoundNodes { get; }
}


public interface IChildBoundNodeProvider
{ 
    IReadOnlyList<IBoundNode> ChildBoundNodes { get; }
}