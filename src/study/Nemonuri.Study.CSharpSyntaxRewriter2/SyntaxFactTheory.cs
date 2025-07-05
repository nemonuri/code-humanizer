namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static class SyntaxFactTheory
{
    public static bool IsIdentifierNameOrLiteralExpression(this ExpressionSyntax expression)
    {
        return expression is IdentifierNameSyntax or LiteralExpressionSyntax;
    }
}