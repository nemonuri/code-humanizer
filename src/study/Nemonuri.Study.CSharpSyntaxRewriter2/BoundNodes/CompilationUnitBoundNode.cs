
namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class CompilationUnitBoundNode : IBoundNode<CompilationUnitSyntax>, IChildBoundNodeProvider<BlockBoundNode>
{

    public CompilationUnitBoundNode(CompilationUnitSyntax syntax)
    {
        Syntax = syntax;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is BlockSyntax)
            .OfType<BlockSyntax>()
            .Select(n => new BlockBoundNode(n, this))
            .ToImmutableArray();
    }

    public ImmutableArray<BlockBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public CompilationUnitSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;
}
