namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public interface IBoundNode
{
    SyntaxNode Syntax { get; }
}

public interface IBoundNode<TSyntaxNode> : IBoundNode 
    where TSyntaxNode : SyntaxNode
{ 
    new TSyntaxNode Syntax { get; }
}
