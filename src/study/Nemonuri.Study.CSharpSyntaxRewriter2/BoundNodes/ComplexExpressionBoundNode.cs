namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ComplexExpressionBoundNode : IExpressionBoundNode,
    IChildBoundNodeProvider<BlockBoundNode>,
    IParentBoundNodeProvider
{
    public static bool TryCreate(ExpressionSyntax syntax, IBoundNode parent, [NotNullWhen(true)] out ComplexExpressionBoundNode? complexExpressionSymbol)
    {
        if (syntax.IsIdentifierNameOrLiteralExpression())
        {
            complexExpressionSymbol = null;
            return false;
        }
        else
        {
            complexExpressionSymbol = new(syntax, parent);
            return true;
        }
    }

    public ComplexExpressionBoundNode(ExpressionSyntax syntax, IBoundNode parent)
    {
        Guard.IsFalse(syntax.IsIdentifierNameOrLiteralExpression());

        Syntax = syntax;
        ParentBoundNode = parent;

        ChildBoundNodes = Syntax.GetDescendantNodes<BlockSyntax>().Select(n => new BlockBoundNode(n, this)).ToArray();
    }

    public ExpressionSyntax Syntax { get; }

    public IBoundNode ParentBoundNode { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;

    public IReadOnlyList<BlockBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;
}
