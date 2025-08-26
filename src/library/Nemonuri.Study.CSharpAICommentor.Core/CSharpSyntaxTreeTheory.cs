
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nemonuri.Failures;
using Nemonuri.Study.CSharpAICommentor.Logging;
using Sf = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nemonuri.Study.CSharpAICommentor;

public static partial class CSharpSyntaxTreeTheory
{
    public static async Task<CreateCSharpSyntaxTreeFromFileResult>
    CreateCSharpSyntaxTreeFromFileAsync
    (
        FileInfo fileInfo,
        ILogger? logger = null,
        CancellationToken cancellationToken = default
    )
    {
        Guard.IsNotNull(fileInfo);
        logger ??= NullLogger.Instance;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogMessageAndMemberWithCaller("Run File.ReadAllTextAsync", fileInfo);

            string text;
            try
            {
                text = await File.ReadAllTextAsync(fileInfo.FullName, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (IsReadAllTextException(e))
            {
                return CreateCSharpSyntaxTreeFromFileResult.CreateAsReadAllTextFailed(e, e.Message);
            }

            logger.LogMessageWithCaller("Invoke CSharpSyntaxTree.ParseText");
            var parsed = CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);
            return CreateCSharpSyntaxTreeFromFileResult.CreateAsValue((CSharpSyntaxTree)parsed);
        }
        catch (OperationCanceledException e)
        {
            return CreateCSharpSyntaxTreeFromFileResult.CreateAsCanceled(e.Message);
        }
    }

    internal static bool IsReadAllTextException(Exception e)
    {
        return e is
            ArgumentException or
            ArgumentNullException or
            PathTooLongException or
            DirectoryNotFoundException or
            IOException or
            UnauthorizedAccessException or
            FileNotFoundException or
            NotSupportedException or
            System.Security.SecurityException
            ;
    }

    public static bool ContainsMissingNodeOrToken(SyntaxNodeOrToken syntaxNodeOrToken)
    {
        bool success =
        TreeNodeAggregatingTheory.TryAggregateAsRoot
        (
            contextFromRootAggregator: new IndexedTreeNodesFromRootAggregator<SyntaxNodeOrToken>(),
            treeNodeAggregator: new AdHocTreeNodeAggregator<SyntaxNodeOrToken, bool>
            (
                defaultSeedProvider: static () => false,
                optionalAggregator: static (context, siblings, children, source) =>
                {
                    if (siblings || children) { return (true, true); }
                    else { return (source.IsMissing, true); }
                }
            ),
            childrenProvider: new AdHocChildrenProvider<SyntaxNodeOrToken>
            (
                static s => s.ChildNodesAndTokens()
            ),
            treeNode: syntaxNodeOrToken,
            out var aggregated
        );

        return success ? aggregated : false;
    }

    public static async Task<CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult>
    CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync
    (
        FileInfo fileInfo,
        ILogger? logger = null,
        CancellationToken cancellationToken = default
    )
    {
        Guard.IsNotNull(fileInfo);
        logger ??= NullLogger.Instance;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            CreateCSharpSyntaxTreeFromFileResult createCSharpSyntaxTreeFromFileResult =
                await CreateCSharpSyntaxTreeFromFileAsync(fileInfo, logger, cancellationToken).ConfigureAwait(false);
            if (createCSharpSyntaxTreeFromFileResult.IsFailure)
            {
                return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsCreateCSharpSyntaxTreeFromFileFailed
                (
                    createCSharpSyntaxTreeFromFileResult.GetFailInfo(),
                    createCSharpSyntaxTreeFromFileResult.GetMessage()
                );
            }

            CSharpSyntaxTree csharpSyntaxTree = createCSharpSyntaxTreeFromFileResult.GetValue();

            logger.LogMessageWithCaller($"Run CSharpSyntaxTree.GetRootAsync");

            CSharpSyntaxNode root;
            try
            {
                root = await csharpSyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsGetRootFailed(e.Message);
            }

            if (root is not CompilationUnitSyntax compilationUnitSyntax)
            {
                return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsRootIsNotCompilationUnitSyntax(root.GetType());
            }
            logger.LogMessageWithCaller($"Confirm root is CompilationUnitSyntax.");

            bool isMissing = ContainsMissingNodeOrToken(compilationUnitSyntax);
            logger.LogMessageAndMemberWithCaller($"{nameof(ContainsMissingNodeOrToken)} invoked.", isMissing);

            return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsValue
            (
                new CompilationUnitRootedCSharpSyntaxTreeInfo
                (
                    csharpSyntaxTree,
                    compilationUnitSyntax,
                    isMissing
                )
            );
        }
        catch (OperationCanceledException e)
        {
            return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsCanceled(e.Message);
        }
    }

    public static bool IsArgumentSyntaxAndHasComplexExpression
    (this SyntaxNode syntaxNode)
    {
        return
            (syntaxNode is ArgumentSyntax argument) &&
            (!(
                argument.Expression is
                IdentifierNameSyntax or
                LiteralExpressionSyntax or
                RangeExpressionSyntax or
                RefExpressionSyntax or
                DefaultExpressionSyntax or
                DeclarationExpressionSyntax
            ));
    }

    public static (SyntaxNode?, int) FindAncestorOrSelf
    (
        this SyntaxNode syntaxNode,
        Func<SyntaxNode, bool> predicate
    )
    {
        Debug.Assert(syntaxNode is not null);
        Debug.Assert(predicate is not null);

        int distanceFromSelf = 0;
        for (SyntaxNode? node = syntaxNode; node is not null; node = node.Parent)
        {
            if (predicate(node))
            {
                return (node, distanceFromSelf);
            }
            distanceFromSelf++;
        }

        return (default, -1);
    }

    private static readonly IChildrenProvider<SyntaxNodeOrToken> s_syntaxNodeOrTokenChildProvider =
        new AdHocChildrenProvider<SyntaxNodeOrToken>
        (
            static a => a.ChildNodesAndTokens()
        );

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
            contextFromRootAggregator: new IndexedTreeNodesFromRootAggregator<SyntaxNodeOrToken>(),
            treeNodeAggregator: new AdHocTreeNodeAggregator<SyntaxNodeOrToken, ImmutableList<RoseTreeNode<IndexSequenceAndSyntaxNode>>>
            (
                defaultSeedProvider: static () => [],
                optionalAggregator: static (context, siblingsAggregated, childrenAggregated, source) =>
                {
                    if
                    (
                        source.AsNode() is { } node1 &&
                        (
                            node1.IsArgumentSyntaxAndHasComplexExpression() ||
                            node1 is BlockSyntax ||
                            node1 is CompilationUnitSyntax
                        )
                    )
                    {
                        RoseTreeNode<IndexSequenceAndSyntaxNode> newNode = new
                        (
                            new IndexSequenceAndSyntaxNode(context.ToIndexSequence<object>(), node1),
                            [.. childrenAggregated]
                        );
                        return (siblingsAggregated.Add(newNode), true);
                    }

                    return (siblingsAggregated.AddRange(childrenAggregated), true);
                }
            ),
            childrenProvider: s_syntaxNodeOrTokenChildProvider,
            treeNode: originalNode,
            aggregated: out ImmutableList<RoseTreeNode<IndexSequenceAndSyntaxNode>>? aggregated
        );

        if (!success) { goto Fail; }
        Debug.Assert(aggregated?.Count == 1);
        RoseTreeNode<IndexSequenceAndSyntaxNode> root2 = aggregated[0];

        success =
        TreeNodeAggregatingTheory.TryAggregateAsRoot
        (
            contextFromRootAggregator: new IndexedRoseTreeNodesFromRootAggregator<IndexSequenceAndSyntaxNode>(),
            treeNodeAggregator: new AdHocRoseTreeNodeAggregator<IndexSequenceAndSyntaxNode, ImmutableList<IndexSequence>>
            (
                defaultSeedProvider: static () => [],
                optionalAggregator: static (context, siblingsAggregated, childrenAggregated, source) =>
                {
                    if
                    (
                        source.Value.SyntaxNode is { } node1 &&
                        node1.IsArgumentSyntaxAndHasComplexExpression()
                    )
                    {
                        return (siblingsAggregated.AddRange(childrenAggregated).Add(source.Value.IndexSequence), true);
                    }
                    else
                    {
                        return (siblingsAggregated.AddRange(childrenAggregated), true);
                    }
                }
            ),
            childrenProvider: new RoseTreeNodeChildrenProvider<IndexSequenceAndSyntaxNode>(),
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
                !s_syntaxNodeOrTokenChildProvider.TryGetDecendentOrSelfAt
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
