namespace Nemonuri.Study.CSharpSyntaxRewriter1;

public static class SyntaxQueryTheory
{
    public static TSyntax? GetEquivalentDescendant<TSyntax>(SyntaxNode root, TSyntax finding)
        where TSyntax : SyntaxNode
    { 
        return root.Contains(finding) ?
            finding :
            (root.DescendantNodes().FirstOrDefault(n => n is TSyntax a1 && a1.IsEquivalentTo(finding)) as TSyntax);
    }
}
