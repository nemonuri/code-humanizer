namespace Nemonuri.Study.CSharpSyntaxRewriter1;

public struct ExpressionInArgumentInBlockStructureRawData
{
    public ExpressionSyntax? ExpressionSyntax;
    public ArgumentSyntax? ArgumentSyntax;
    public BlockSyntax? BlockSyntax;

    public readonly bool IsSatisfyingStructure =>
        (ExpressionSyntax, ArgumentSyntax, BlockSyntax) is ({ } e, { } a, { } b) &&
        CSharpSyntaxRelationTheory.IsExpressionInArgumentInBlockStructure(e, a, b);
}