namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal class TriviaAdder : CSharpSyntaxRewriter
{
    public TriviaAdder() : base(visitIntoStructuredTrivia: false)
    { }

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        return WithTrivia(base.VisitExpressionStatement(node));

        /*
                string triviaChunk = newNode.GetTrailingTrivia().ToFullString();
                string[] lineSplitted = RegexTheory.GetLineBreakRegex().Split(triviaChunk);

                if (lineSplitted.Length <= 0) { return null; }

                string insertingTrivia = lineSplitted[^1] + "//";

                string[] newSplitted = new string[lineSplitted.Length];
                for (int i = 0; i < newSplitted.Length; i++)
                {
                    if (i == newSplitted.Length - 1)
                    {
                        newSplitted[i] = lineSplitted[^1];
                    }
                    else if (i == newSplitted.Length - 2)
                    {
                        newSplitted[i] = insertingTrivia;
                    }
                    else
                    {
                        newSplitted[i] = lineSplitted[i];
                    }
                }
        */
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
        return node
                    .WithLeadingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, "//"))
                    //.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, Environment.NewLine + Environment.NewLine))
                    ;
                    
    }
}
