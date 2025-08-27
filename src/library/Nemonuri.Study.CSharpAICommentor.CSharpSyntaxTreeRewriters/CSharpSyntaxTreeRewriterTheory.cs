
using Sf = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nemonuri.Study.CSharpAICommentor.CSharpSyntaxTreeRewriters;


public static class CSharpSyntaxNodeRewritingTheory
{
    public static bool
    TrySeparateComplexArgumentExpressions
    (
        CompilationUnitSyntax originalNode,
        [NotNullWhen(true)] out CSharpSyntaxNode? result
    )
    {
        bool success;

        success =
        TreeNodeAggregatingTheory.TryAggregateAsRoot
        (
            contextFromRootAggregator: Constants.s_syntaxNodeOrTokenTypedIndexedTreeNodesFromRootAggregator,
            treeNodeAggregator: Constants.s_syntaxNodeOrTokenToIndexesFromRootPairedSyntaxNodeTreeAggregator,
            childrenProvider: Constants.s_syntaxNodeOrTokenChildrenProvider,
            treeNode: originalNode,
            aggregated: out ImmutableList<RoseTreeNode<IndexesFromRootPairedSyntaxNode>>? aggregated
        );

        if (!success) { goto Fail; }
        Debug.Assert(aggregated?.Count == 1);
        RoseTreeNode<IndexesFromRootPairedSyntaxNode> root2 = aggregated[0];

        success =
        TreeNodeAggregatingTheory.TryAggregateAsRoot
        (
            contextFromRootAggregator: Constants.s_indexesFromRootPairedSyntaxNodeTreeContextAggregator,
            treeNodeAggregator: Constants.s_indexesFromRootPairedSyntaxNodeTreeNodeAggregator,
            childrenProvider: Constants.s_indexesFromRootPairedSyntaxNodeTreeChildrenProvider,
            treeNode: root2,
            aggregated: out ImmutableList<IndexSequence>? aggregated2
        );

        if (!success) { goto Fail; }
        Debug.Assert(aggregated2 is not null);

        IndexSequence[] sortedIndexSequences = aggregated2.Sort(Int32ReadOnlyListCompareTheory.CompareFromDownLeftToTopRight).ToArray();
        result = CreateComplexArgumentExpressionsSeparatedSyntaxNode(originalNode, sortedIndexSequences);
        return true;

    Fail:
        result = default;
        return false;
    }

    internal static CSharpSyntaxNode CreateComplexArgumentExpressionsSeparatedSyntaxNode
    (
        CompilationUnitSyntax originalCompilationUnitSyntax,
        IndexSequence[] sortedIndexSequences
    )
    {
        Debug.Assert(originalCompilationUnitSyntax is not null);
        Debug.Assert(true /* TODO: indexSequences is sorted */);

        CompilationUnitSyntax compilationUnitSyntax = originalCompilationUnitSyntax;
        const string replacementVariableNamePrefix = "v_";
        int replacementVariableNameSuffix = 0;

        for (int i = 0; i < sortedIndexSequences.Length; i++)
        {
            IndexSequence indexSequence = sortedIndexSequences[i];

            if
            (
                !Constants.s_syntaxNodeOrTokenChildrenProvider.TryGetDecendentOrSelfAt
                (
                    compilationUnitSyntax,
                    indexSequence,
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

            IndexSequence statementIndexSequence = indexSequence[..^distance];

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
            ).NormalizeWhitespace();

            //--- format insertingStatement ---
            //insertingStatement = insertingStatement
            //---|

            var replacementStatement = statement.ReplaceNode(sourceArgumentExpression, replacementArgumentExpression);
            var replacementStatements = block.Statements
                                            .RemoveAt(statementIndex)
                                            .InsertRange(statementIndex, [insertingStatement, replacementStatement]);

            compilationUnitSyntax = compilationUnitSyntax.ReplaceNode(block, block.WithStatements(replacementStatements));

            for (int i2 = i + 1; i2 < sortedIndexSequences.Length; i2++)
            {
                var v1 = sortedIndexSequences[i2];
                var v2 = v1.UpdateAsInserted(statementIndexSequence, 1);
                sortedIndexSequences[i2] = v2;
            }
        }

        Debug.Assert(compilationUnitSyntax is not null);
        return compilationUnitSyntax;
    }
}
