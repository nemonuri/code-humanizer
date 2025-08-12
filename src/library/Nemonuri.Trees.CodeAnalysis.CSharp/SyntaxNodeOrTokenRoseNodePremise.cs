
namespace Nemonuri.Trees.CodeAnalysis.CSharp;

public class SyntaxNodeOrTokenChildProvider : IChildrenProvider<SyntaxNodeOrToken>
{
    public IEnumerable<SyntaxNodeOrToken> GetChildren(SyntaxNodeOrToken source)
    {
        return source.ChildNodesAndTokens();
    }
}
