namespace Nemonuri.Study.CSharpSyntaxRewriter1;

public static class BlockSyntaxTheory
{
    public static int GetIndexOfAncestorStatement(this BlockSyntax block, SyntaxNode node)
    {
        if (!block.IsAncestorOf(node)) { return -1; }

        StatementSyntax? statement = node is StatementSyntax v1 ? v1 : node.FirstAncestorOrSelf<StatementSyntax>(); 
        while (statement is not null)
        {
            int result = block.Statements.IndexOf(statement);
            if (result >= 0) { return result; }

            statement = statement.FirstAncestorOrSelf<StatementSyntax>(n => statement != n);
        }

        return -1;
    }

    public static int GetInsertingIndex(this BlockSyntax block, SyntaxNode nodeInStatements)
    {
        int indexOfAncestor = block.GetIndexOfAncestorStatement(nodeInStatements);
        return indexOfAncestor < 0 ? 0 : indexOfAncestor;
    }
}