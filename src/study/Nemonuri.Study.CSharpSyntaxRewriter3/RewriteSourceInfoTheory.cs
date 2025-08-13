namespace Nemonuri.Study.CSharpSyntaxRewriter3;

using Sf = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal static class RewriteSourceInfoTheory
{
    private static readonly SyntaxNodeOrTokenChildProvider s_syntaxNodeOrTokenChildProvider = new();

    public static SyntaxTree CreateRewritedSyntaxTree
    (
        RewriteSourceInfo[] sortedRewriteSourceInfos
    )
    {
        Debug.Assert(true /* TODO: sortedRewriteSourceInfos is sorted */);

        CompilationUnitSyntax? compilationUnitSyntax = null;
        const string replacementVariableNamePrefix = "v_";
        int replacementVariableNameSuffix = 0;

        for (int i = 0; i < sortedRewriteSourceInfos.Length; i++)
        {
            var rsInfo = sortedRewriteSourceInfos[i];

            if (compilationUnitSyntax is null)
            {
                var (syntax, _) = rsInfo.SyntaxNode.FindAncestorOrSelf(static a => a is CompilationUnitSyntax);
                compilationUnitSyntax = syntax as CompilationUnitSyntax;

                Guard.IsNotNull(compilationUnitSyntax);
            }

            //SyntaxNodeOrToken sourceSyntaxNodeOrToken = compilationUnitSyntax;
            if
            (
                !s_syntaxNodeOrTokenChildProvider.TryGetDecendentOrSelfAt
                (
                    compilationUnitSyntax,
                    rsInfo.IndexSequence,
                    out var decendentOrSelf
                )
            )
            {
                throw new InvalidOperationException(/* TODO */);
            }

            if
            (
                !(decendentOrSelf.AsNode() is { } sourceSyntaxNode &&
                sourceSyntaxNode.IsArgumentSyntaxAndHasComplexExpression())
            )
            { continue; }

            var (maybeStatement, distance) = sourceSyntaxNode.FindAncestorOrSelf
            (
                static a => a is StatementSyntax && a.Parent is BlockSyntax
            );

            if (maybeStatement is not StatementSyntax statement) { continue; }

            IndexSequence statementIndexSequence = rsInfo.IndexSequence[..^distance];

            BlockSyntax? block = statement.Parent as BlockSyntax;
            Debug.Assert(block is not null);

            int statementIndex = block.Statements.IndexOf(statement);
            Debug.Assert(statementIndex >= 0);

            string replacementVariableName = replacementVariableNamePrefix + replacementVariableNameSuffix;
            replacementVariableNameSuffix++;

            var sourceArgumentExpression = ((ArgumentSyntax)sourceSyntaxNode).Expression;
            var replacementArgumentExpression = Sf.IdentifierName(replacementVariableName);
            var varExpression = Sf.IdentifierName("var");

            var insertingStatement = Sf.LocalDeclarationStatement
            (
                Sf.VariableDeclaration
                (
                    type: varExpression, //.WithTrailingTrivia(Sf.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ")),
                    variables: Sf.SingletonSeparatedList
                    (
                        Sf.VariableDeclarator
                        (
                            identifier: replacementArgumentExpression.Identifier,
                            argumentList: default,
                            initializer: Sf.EqualsValueClause
                            (
                                value: sourceArgumentExpression
                            )
                        )
                    )
                )
            );
            var replacementStatement = statement.ReplaceNode(sourceArgumentExpression, replacementArgumentExpression);
            var replacementStatements = block.Statements
                                            .RemoveAt(statementIndex)
                                            .InsertRange(statementIndex, [insertingStatement, replacementStatement]);

            compilationUnitSyntax = compilationUnitSyntax.ReplaceNode(block, block.WithStatements(replacementStatements));

            for (int i2 = i + 1; i2 < sortedRewriteSourceInfos.Length; i2++)
            {
                var v1 = sortedRewriteSourceInfos[i2];
                var v2 = v1.WithIndexSequence(v1.IndexSequence.UpdateAsInserted(statementIndexSequence, 1));
                sortedRewriteSourceInfos[i2] = v2;
            }
        }

        Debug.Assert(compilationUnitSyntax is not null);
        return compilationUnitSyntax.NormalizeWhitespace().SyntaxTree;
    }
}