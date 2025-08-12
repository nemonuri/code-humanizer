
namespace Nemonuri.Trees.CodeAnalysis.CSharp;

public class SyntaxNodeOrTokenChildProvider : IChildrenProvider<SyntaxNodeOrToken>
{
    public SyntaxNodeOrTokenChildProvider() { }

    public IEnumerable<SyntaxNodeOrToken> GetChildren(SyntaxNodeOrToken source)
    {
        return source.ChildNodesAndTokens();
    }
}
