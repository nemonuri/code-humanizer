namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class DefaultExpressionBoundNode : IExpressionBoundNode,
    IParentBoundNodeProvider
{
    public DefaultExpressionBoundNode(ExpressionSyntax syntax, IBoundNode parentBoundNode)
    {
        Syntax = syntax;
        ParentBoundNode = parentBoundNode;
    }

    public ExpressionSyntax Syntax { get; }
    public IBoundNode ParentBoundNode { get; }
    SyntaxNode IBoundNode.Syntax => Syntax;
}
