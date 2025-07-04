namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public interface IBoundNode
{
    SyntaxNode Syntax { get; }
}

public interface IBoundNode<TSyntaxNode> : IBoundNode 
    where TSyntaxNode : SyntaxNode
{ 
    new TSyntaxNode Syntax { get; }
}

public interface IParentBoundNodeProvider<TBoundNode> : IParentBoundNodeProvider
    where TBoundNode : IBoundNode
{
    new TBoundNode ParentBoundNode { get; }
}

public interface IParentBoundNodeProvider
{ 
    IBoundNode ParentBoundNode { get; }
}

public interface IChildBoundNodeProvider<TBoundNode> : IChildBoundNodeProvider
    where TBoundNode : IBoundNode
{
    new ImmutableArray<TBoundNode> ChildBoundNodes { get; }
}

public interface IChildBoundNodeProvider
{ 
    IReadOnlyList<IBoundNode> ChildBoundNodes { get; }
}