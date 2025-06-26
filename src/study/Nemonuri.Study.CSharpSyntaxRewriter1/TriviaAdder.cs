namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal class TriviaAdder : CSharpSyntaxRewriter
{
    public TriviaAdder() : base(visitIntoStructuredTrivia: false)
    { }

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        return WithTrivia(base.VisitExpressionStatement(node));
    }

    public override SyntaxNode? VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        return WithTrivia(base.VisitLocalDeclarationStatement(node));
    }

    public override SyntaxNode? VisitReturnStatement(ReturnStatementSyntax node)
    {
        return WithTrivia(base.VisitReturnStatement(node));
    }

    private static SyntaxNode? WithTrivia(SyntaxNode? node)
    {
        if (node is null) { return null; }
        return node.WithLeadingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, "//"));
    }
}
