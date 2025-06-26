
using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal class ArgumentToLocalVarRewriter : CSharpSyntaxRewriter
{
    private readonly Stack<BlockContext> _blockContextStack = new();
    private readonly Stack<ArgumentContext> _argumentContextStack = new();

    public ArgumentToLocalVarRewriter() : base(visitIntoStructuredTrivia: false)
    { }

    public override SyntaxNode? VisitBlock(BlockSyntax node)
    {
        return base.VisitBlock(node);
    }

    public override SyntaxNode? VisitArgument(ArgumentSyntax node)
    {
        return base.VisitArgument(node);
    }

    [return: NotNullIfNotNull(nameof(node))]
    public override SyntaxNode? Visit(SyntaxNode? node)
    {
        if
        (
            node is ExpressionSyntax expression &&
            !CSharpSyntaxRelationTheory.IsIdentifierNameOrLiteralExpression(expression) &&

            _argumentContextStack.TryPeek(out ArgumentContext? argumentContext) &&
            argumentContext is not null &&
            argumentContext.Argument.IsAncestorOf(expression) &&

            _blockContextStack.TryPeek(out BlockContext? blockContext) &&
            blockContext is not null &&
            blockContext.Block.IsAncestorOf(argumentContext.Argument)
        )
        {
            //--- 트리 관계 형성 ---
            if (argumentContext.ParentBlockContext is null)
            {
                argumentContext.ParentBlockContext = blockContext;
                blockContext.ChildArgumentContexts.Add(argumentContext);
            }
            //---|

            
        }

        return base.Visit(node);
    }


    private class BlockContext
    {
        public BlockSyntax Block { get; }

        private readonly List<InsertingStatementRawData> _insertingStatementDatas = new();

        public List<ArgumentContext> ChildArgumentContexts { get; } = new();

        public BlockContext(BlockSyntax block)
        {
            Block = block;
        }
    }

    private class ArgumentContext
    {
        public ArgumentSyntax Argument { get; }

        public ArgumentContext(ArgumentSyntax argument)
        {
            Argument = argument;
        }

        public BlockContext? ParentBlockContext { get; set; }
    }

    internal struct InsertingStatementRawData
    {
        public int OriginalStatementIndex;
        public LocalDeclarationStatementSyntax? InsertingLocalDeclaration;
    }
}
