namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public interface IParentBoundNodeProvider<TBoundNode> : IParentBoundNodeProvider
    where TBoundNode : IBoundNode
{
    new TBoundNode ParentBoundNode { get; }
}

public interface IParentBoundNodeProvider
{ 
    IBoundNode ParentBoundNode { get; }
}