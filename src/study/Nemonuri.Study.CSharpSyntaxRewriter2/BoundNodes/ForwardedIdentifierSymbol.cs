
namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ForwardedIdentifierSymbol : IBoundNode<IdentifierNameSyntax>,
    IParentBoundNodeProvider,
    IExpressionBoundNode
{
    public ForwardedIdentifierSymbol(IdentifierNameSyntax syntax, IBoundNode parentBoundNode)
    {
        Syntax = syntax;
        ParentBoundNode = parentBoundNode;
    }

    public IdentifierNameSyntax Syntax { get; }
    public IBoundNode ParentBoundNode { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;

    ExpressionSyntax IBoundNode<ExpressionSyntax>.Syntax => Syntax;
}