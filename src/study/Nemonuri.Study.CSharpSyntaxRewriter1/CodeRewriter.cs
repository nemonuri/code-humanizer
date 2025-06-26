using System.Diagnostics.CodeAnalysis;
using S = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal class CodeRewriter : CSharpSyntaxRewriter
{
    private readonly Stack<BlockContext> _blockContextStack = new();
    private int _currentBlockNumber = 0;
    private Mode _mode;

    public CodeRewriter() : base(visitIntoStructuredTrivia: false)
    { }

    public SyntaxNode? Run(SyntaxNode? root)
    {
        if (root is null) { return null; }

        SyntaxNode newRoot = root;

        _mode = Mode.IfStatementExpressionToLocalDeclartion;
        newRoot = Visit(newRoot);

        _mode = Mode.ArgumentExpressionToLocalDeclartion;
        newRoot = Visit(newRoot);

        return newRoot;
    }

    public override SyntaxNode? VisitBlock(BlockSyntax block)
    {
        _blockContextStack.Push(new BlockContext(block));
        var newSyntax = base.VisitBlock(block);
        var blockContext = _blockContextStack.Pop();

        if (newSyntax is not BlockSyntax newBlock)
        {
            return newSyntax;
        }

        //--- apply Changes ---
        if (_mode == Mode.IfStatementExpressionToLocalDeclartion)
        {
            foreach (IfStatementExpressionToLocalDeclartionCodeFixData c in blockContext.IfStatementExpressionToLocalDeclartionCodeFixDatas)
            {
                if (SyntaxQueryTheory.GetEquivalentDescendant(newBlock, c.IfStatement.Condition)?.Parent is not IfStatementSyntax matched) { continue; }

                int insertingIndex = newBlock.GetInsertingIndex(matched);
                newBlock = newBlock.ReplaceNode(matched.Condition, c.IdentifierNameAsNewArgumentExpression);
                newBlock = newBlock.WithStatements(newBlock.Statements.Insert(insertingIndex, c.InsertingLocalDeclarationStatement));
            }
        }

        if (_mode == Mode.ArgumentExpressionToLocalDeclartion)
        {
            foreach (ArgumentExpressionToLocalDeclartionCodeFixData c in blockContext.ArgumentExpressionToLocalDeclartionCodeFixDatas)
            {
                if (SyntaxQueryTheory.GetEquivalentDescendant(newBlock, c.Argument) is not { } matched) { continue; }

                int insertingIndex = newBlock.GetInsertingIndex(matched);
                newBlock = newBlock.ReplaceNode(matched.Expression, c.IdentifierNameAsNewArgumentExpression);
                newBlock = newBlock.WithStatements(newBlock.Statements.Insert(insertingIndex, c.InsertingLocalDeclarationStatement));
            }
        }
        //---|

        return newBlock;
    }

    public override SyntaxNode? VisitArgument(ArgumentSyntax argument)
    {
        if (IsSatisfyingCondition(this, argument, out var blockContext))
        {
            //--- blockContext 에 번호 할당 ---
            blockContext.BlockNumber ??= Interlocked.Increment(ref _currentBlockNumber);
            //---|

            //--- Get block·argument number ---
            if (blockContext.BlockNumber is not int blockNumber) { goto DefaultResult; }
            int argumentNumber = blockContext.ArgumentExpressionToLocalDeclartionCodeFixDatas.Count + 1;
            //---|

            //--- Create local varible name ---
            string localVaribleName = $"v{blockNumber}_{argumentNumber}";
            //---|

            ArgumentExpressionToLocalDeclartionCodeFixData codeChangeData = new()
            {
                Argument = argument,
                IdentifierNameAsNewArgumentExpression = S.IdentifierName(localVaribleName),
                InsertingLocalDeclarationStatement = S.LocalDeclarationStatement
                (
                    S.VariableDeclaration
                    (
                        S.IdentifierName("var"),
                        S.SingletonSeparatedList
                        (
                            S.VariableDeclarator
                            (
                                S.Identifier(localVaribleName),
                                default,
                                S.EqualsValueClause(argument.Expression.WalkDownParentheses())
                            )
                        )
                    )
                )
            };

            blockContext.ArgumentExpressionToLocalDeclartionCodeFixDatas.Add(codeChangeData);
            return argument;
        }

    DefaultResult:
        return base.VisitArgument(argument);

        static bool IsSatisfyingCondition
        (
            CodeRewriter self,
            ArgumentSyntax argument,
            [NotNullWhen(true)] out BlockContext? blockContext
        )
        {
            var expression = argument.Expression;
            blockContext = null;

            if (self._mode != Mode.ArgumentExpressionToLocalDeclartion) { return false; }

            if (CSharpSyntaxRelationTheory.IsIdentifierNameOrLiteralExpression(expression))
            {
                return false;
            }

            if
            (
                !self._blockContextStack.TryPeek(out blockContext) ||
                blockContext is null
            )
            {
                return false;
            }

            if (blockContext.Block != argument.FirstAncestorOrSelf<BlockSyntax>())
            {
                return false;
            }

            return true;
        }
    }

    public override SyntaxNode? VisitIfStatement(IfStatementSyntax ifStatement)
    {
        if (IsSatisfyingCondition(this, ifStatement, out var blockContext))
        {
            //--- blockContext 에 번호 할당 ---
            blockContext.BlockNumber ??= Interlocked.Increment(ref _currentBlockNumber);
            //---|

            //--- Get block·argument number ---
            if (blockContext.BlockNumber is not int blockNumber) { goto DefaultResult; }
            int argumentNumber = blockContext.IfStatementExpressionToLocalDeclartionCodeFixDatas.Count + 1;
            //---|

            //--- Create local varible name ---
            string localVaribleName = $"b{blockNumber}_{argumentNumber}";
            //---|

            IfStatementExpressionToLocalDeclartionCodeFixData codeFixData = new()
            {
                IfStatement = ifStatement,
                IdentifierNameAsNewArgumentExpression = S.IdentifierName(localVaribleName),
                InsertingLocalDeclarationStatement = S.LocalDeclarationStatement
                (
                    S.VariableDeclaration
                    (
                        S.PredefinedType(S.Token(SyntaxKind.BoolKeyword)),
                        S.SingletonSeparatedList
                        (
                            S.VariableDeclarator
                            (
                                S.Identifier(localVaribleName),
                                default,
                                S.EqualsValueClause(ifStatement.Condition.WalkDownParentheses())
                            )
                        )
                    )
                )
            };

            blockContext.IfStatementExpressionToLocalDeclartionCodeFixDatas.Add(codeFixData);
        }

    DefaultResult:
        return base.VisitIfStatement(ifStatement);

        static bool IsSatisfyingCondition
        (
            CodeRewriter self,
            IfStatementSyntax ifStatement,
            [NotNullWhen(true)] out BlockContext? blockContext
        )
        {
            var expression = ifStatement.Condition;
            blockContext = null;

            if (self._mode != Mode.IfStatementExpressionToLocalDeclartion) { return false; }

            if (CSharpSyntaxRelationTheory.IsIdentifierNameOrLiteralExpression(expression))
            {
                return false;
            }

            if
            (
                !self._blockContextStack.TryPeek(out blockContext) ||
                blockContext is null
            )
            {
                return false;
            }

            if (blockContext.Block != ifStatement.FirstAncestorOrSelf<BlockSyntax>())
            {
                return false;
            }

            return true;
        }
    }

    private enum Mode
    {
        IfStatementExpressionToLocalDeclartion = 0,
        ArgumentExpressionToLocalDeclartion = 1
    }

    private class BlockContext
    {
        public BlockSyntax Block { get; }

        public int? BlockNumber { get; set; }

        public List<ArgumentExpressionToLocalDeclartionCodeFixData> ArgumentExpressionToLocalDeclartionCodeFixDatas { get; } = new();

        public List<IfStatementExpressionToLocalDeclartionCodeFixData> IfStatementExpressionToLocalDeclartionCodeFixDatas { get; } = new();

        public BlockContext(BlockSyntax block)
        {
            Block = block;
        }
    }

    private class ArgumentExpressionToLocalDeclartionCodeFixData
    {
        public ArgumentExpressionToLocalDeclartionCodeFixData() { }

        public required ArgumentSyntax Argument;

        public required IdentifierNameSyntax IdentifierNameAsNewArgumentExpression;

        public required LocalDeclarationStatementSyntax InsertingLocalDeclarationStatement;
    }

    private class IfStatementExpressionToLocalDeclartionCodeFixData
    {
        public IfStatementExpressionToLocalDeclartionCodeFixData() { }

        public required IfStatementSyntax IfStatement;

        public required IdentifierNameSyntax IdentifierNameAsNewArgumentExpression;

        public required LocalDeclarationStatementSyntax InsertingLocalDeclarationStatement;
    }
}
