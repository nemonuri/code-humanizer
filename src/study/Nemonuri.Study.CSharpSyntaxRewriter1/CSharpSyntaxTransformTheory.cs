namespace Nemonuri.Study.CSharpSyntaxRewriter1;

public static class CSharpSyntaxTransformTheory
{
    public static IReadOnlyList<ExpressionInArgumentInBlockStructureRawData> GetExpressionInArgumentInBlockStructureRawDatas(CSharpSyntaxNode syntaxNode)
    {
        CSharpSyntaxWalkerForExpressionInArgumentInBlockStructureRawData walker = new();
        walker.Visit(syntaxNode);
        return walker.Results;
    }

    private static int _suffix = 0;

    public static BlockSyntax TransformToLocalDeclare
    (
        ExpressionSyntax expression,
        ArgumentSyntax argument,
        BlockSyntax block
    )
    {
        string localVarName = "v" + _suffix++;

        var ldSynax = SyntaxFactory.LocalDeclarationStatement
        (
            SyntaxFactory.VariableDeclaration
            (
                SyntaxFactory.IdentifierName
                (
                    SyntaxFactory.Identifier("var")
                ),
                SyntaxFactory.SingletonSeparatedList
                (
                    SyntaxFactory.VariableDeclarator
                    (
                        SyntaxFactory.Identifier(localVarName),
                        null,
                        SyntaxFactory.EqualsValueClause(expression)
                    )
                )
            )
        );

        var newArgument = argument.WithExpression(SyntaxFactory.IdentifierName(localVarName));

        int argumentParentStatementIndex = -1;
        if (argument.FirstAncestorOrSelf<StatementSyntax>() is { } argumentParentStatement)
        {
            argumentParentStatementIndex = block.Statements.IndexOf(argumentParentStatement);
        }

        if (argumentParentStatementIndex >= 0)
        {
            block = block.WithStatements
            (
                block.Statements.Replace
                (
                    block.Statements[argumentParentStatementIndex],
                    block.Statements[argumentParentStatementIndex].ReplaceNode(argument, newArgument)
                )
            );
        }

        argumentParentStatementIndex = Math.Max(argumentParentStatementIndex, 0);

        return block.WithStatements(block.Statements.Insert(argumentParentStatementIndex, ldSynax));
    }
}