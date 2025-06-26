using System.Diagnostics.CodeAnalysis;
using S = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal class ArgumentToLocalVarRewriter : CSharpSyntaxRewriter
{
    private readonly Stack<BlockContext> _blockContextStack = new();
    private int _currentBlockNumber = 0;

    public ArgumentToLocalVarRewriter() : base(visitIntoStructuredTrivia: false)
    { }

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
        foreach (CodeChangeData c in blockContext.CodeChangeDatas)
        {
            newBlock = newBlock.ReplaceNode(c.Argument.Expression, c.IdentifierNameAsNewArgumentExpression);

            int insertingIndex = newBlock.GetInsertingIndex(c.Argument);
            newBlock = newBlock.WithStatements(newBlock.Statements.Insert(insertingIndex, c.InsertingLocalDeclarationStatement));
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
            int argumentNumber = blockContext.CodeChangeDatas.Count + 1;
            //---|

            //--- Create local varible name ---
            string localVaribleName = $"v{blockNumber}_{argumentNumber}";
            //---|

            CodeChangeData codeChangeData = new()
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

            blockContext.CodeChangeDatas.Add(codeChangeData);
            return argument;
        }

DefaultResult:
        return base.VisitArgument(argument);

        static bool IsSatisfyingCondition
        (
            ArgumentToLocalVarRewriter self,
            ArgumentSyntax argument,
            [NotNullWhen(true)] out BlockContext? blockContext
        )
        {
            var expression = argument.Expression;
            blockContext = null;

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

    private class BlockContext
    {
        public BlockSyntax Block { get; }

        public int? BlockNumber { get; set; }

        public List<CodeChangeData> CodeChangeDatas { get; } = new();

        public BlockContext(BlockSyntax block)
        {
            Block = block;
        }
    }

    private class CodeChangeData
    {
        public CodeChangeData() { }

        public required ArgumentSyntax Argument;

        public required IdentifierNameSyntax IdentifierNameAsNewArgumentExpression;

        public required LocalDeclarationStatementSyntax InsertingLocalDeclarationStatement;
    }
}
