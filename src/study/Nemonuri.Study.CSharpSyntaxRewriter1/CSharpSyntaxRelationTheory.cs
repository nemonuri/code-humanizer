namespace Nemonuri.Study.CSharpSyntaxRewriter1;

/// <summary>
/// 구문 사이의 관계에 대한 이론입니다.
/// </summary>
public static class CSharpSyntaxRelationTheory
{
    /// <summary>
    /// 표현 구문이 <see cref="IdentifierNameSyntax"/> 인지 <see cref="LiteralExpressionSyntax"/> 인지 확인합니다.
    /// </summary>
    /// <param name="expression">표현 구문</param>
    /// <returns>확인 결과</returns>
    public static bool IsIdentifierNameOrLiteralExpression(ExpressionSyntax expression)
    {
        return expression is IdentifierNameSyntax or LiteralExpressionSyntax;
    }

    /// <summary>
    /// 블록 구문 안에 인자 구문 안에 표현 구문이 있는 구조인지 확인합니다.
    /// </summary>
    /// <param name="expression">표현 구문</param>
    /// <param name="argument">인자 구문</param>
    /// <param name="block">블록 구문</param>
    /// <returns>확인 결과</returns>
    public static bool IsExpressionInArgumentInBlockStructure
    (
        ExpressionSyntax expression,
        ArgumentSyntax argument,
        BlockSyntax block
    )
    {
        return block.IsAncestorOf(argument) && argument.IsAncestorOf(expression);
    }

    /// <summary>
    /// 이 구문이 대상 구문의 조상인지 확인합니다.
    /// </summary>
    /// <param name="maybeAncestor">조상으로 예상되는 구문</param>
    /// <param name="maybeDescendant">후손으로 예상되는 구문</param>
    /// <returns>확인 결과</returns>
    public static bool IsAncestorOf(this CSharpSyntaxNode maybeAncestor, CSharpSyntaxNode maybeDescendant)
    {
        SyntaxNode? currentNode = maybeDescendant;
        while ((currentNode = currentNode?.Parent) is not null)
        {
            if (currentNode == maybeAncestor)
            {
                return true;
            }
        }
        return false;
    }

    public static IReadOnlyList<ExpressionInArgumentInBlockStructureRawData> GetExpressionInArgumentInBlockStructureRawDatas(CSharpSyntaxNode syntaxNode)
    {
        CSharpSyntaxWalkerForEbpPair walker = new();
        walker.Visit(syntaxNode);
        return walker.Results;
    }
}

internal class CSharpSyntaxWalkerForEbpPair : CSharpSyntaxWalker
{
    private readonly List<ExpressionInArgumentInBlockStructureRawData> _results = new();
    private readonly Stack<BlockSyntax> _blockStack = new();
    private readonly Stack<ArgumentSyntax> _argumentStack = new();

    public CSharpSyntaxWalkerForEbpPair() : base(SyntaxWalkerDepth.Node)
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
        base.Visit(node);

        if
        (
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
        }
    }
}