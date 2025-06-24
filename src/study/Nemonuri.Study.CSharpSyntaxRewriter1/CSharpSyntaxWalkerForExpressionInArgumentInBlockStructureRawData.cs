namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal class CSharpSyntaxWalkerForExpressionInArgumentInBlockStructureRawData : CSharpSyntaxWalker
{
    private readonly List<ExpressionInArgumentInBlockStructureRawData> _results = new();
    private readonly Stack<BlockSyntax> _blockStack = new();
    private readonly Stack<ArgumentSyntax> _argumentStack = new();
    private bool _enableAddingResult = true;

    public CSharpSyntaxWalkerForExpressionInArgumentInBlockStructureRawData() : base(SyntaxWalkerDepth.Node)
    {
    }

    public IReadOnlyList<ExpressionInArgumentInBlockStructureRawData> Results => _results;

    public override void VisitBlock(BlockSyntax block)
    {
        _blockStack.Push(block);
        base.VisitBlock(block);
        _blockStack.Pop();
    }

    public override void VisitArgument(ArgumentSyntax argument)
    {
        _argumentStack.Push(argument);
        base.VisitArgument(argument);
        _argumentStack.Pop();
    }

    public override void Visit(SyntaxNode? node)
    {
        bool restore = false;

        if
        (
            _enableAddingResult &&
            node is ExpressionSyntax expression &&
            !CSharpSyntaxRelationTheory.IsIdentifierNameOrLiteralExpression(expression) &&
            _argumentStack.TryPeek(out ArgumentSyntax? argument) &&
            _blockStack.TryPeek(out BlockSyntax? block)
        )
        {
            _results.Add
            (
                new ExpressionInArgumentInBlockStructureRawData()
                {
                    ExpressionSyntax = expression,
                    ArgumentSyntax = argument,
                    BlockSyntax = block
                }
            );
            _enableAddingResult = false;
            restore = true;
        }

        base.Visit(node);

        if (restore)
        {
            _enableAddingResult = true;
        }
    }
}