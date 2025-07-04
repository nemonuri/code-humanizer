
namespace Nemonuri.Study.CSharpSyntaxRewriter2;

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

public class BlockBoundNode : IBoundNode<BlockSyntax>, IChildBoundNodeProvider<StatementBoundNode>, IParentBoundNodeProvider
{

    public BlockBoundNode(BlockSyntax syntax, IBoundNode parent)
    {
        Syntax = syntax;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is StatementSyntax)
            .OfType<StatementSyntax>()
            .Select(n => new StatementBoundNode(n, this))
            .ToImmutableArray();
        ParentBoundNode = parent;
    }

    public ImmutableArray<StatementBoundNode> ChildBoundNodes { get; }

    public BlockSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;

    public IBoundNode ParentBoundNode { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;
}

public class StatementBoundNode : IBoundNode<StatementSyntax>,
    IChildBoundNodeProvider<ArgumentListBoundNode>,
    IParentBoundNodeProvider<BlockBoundNode>
{
    public StatementBoundNode(StatementSyntax syntax, BlockBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is ArgumentListSyntax)
            .OfType<ArgumentListSyntax>()
            .Select(n => new ArgumentListBoundNode(n, this))
            .ToImmutableArray();
    }

    public BlockBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public ImmutableArray<ArgumentListBoundNode> ChildBoundNodes { get; }


    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public StatementSyntax Syntax { get; }


    SyntaxNode IBoundNode.Syntax => Syntax;
}

public class ArgumentListBoundNode : IBoundNode<ArgumentListSyntax>,
    IChildBoundNodeProvider<ArgumentBoundNode>,
    IParentBoundNodeProvider<StatementBoundNode>
{
    public ArgumentListBoundNode(ArgumentListSyntax syntax, StatementBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is ArgumentSyntax)
            .OfType<ArgumentSyntax>()
            .Select(n => new ArgumentBoundNode(n, this))
            .ToImmutableArray();
    }

    public StatementBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public ImmutableArray<ArgumentBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public ArgumentListSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;
}

public class ArgumentBoundNode : IBoundNode<ArgumentSyntax>,
    IChildBoundNodeProvider<BlockBoundNode>,
    IParentBoundNodeProvider<ArgumentListBoundNode>
{
    public ArgumentBoundNode(ArgumentSyntax syntax, ArgumentListBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is BlockSyntax)
            .OfType<BlockSyntax>()
            .Select(n => new BlockBoundNode(n, this))
            .ToImmutableArray();
    }

    public ArgumentListBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public ImmutableArray<BlockBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public ArgumentSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;
}
