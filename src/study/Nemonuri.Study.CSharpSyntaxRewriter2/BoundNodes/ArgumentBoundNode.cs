namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ArgumentBoundNode : IBoundNode<ArgumentSyntax>,
    IChildBoundNodeProvider<IExpressionBoundNode>,
    IParentBoundNodeProvider<ArgumentListBoundNode>
{
    internal readonly IExpressionBoundNode[] _childBoundNodes;

    public ArgumentBoundNode(ArgumentSyntax syntax, ArgumentListBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;

        var expression = Syntax.Expression.WalkDownParentheses();
        if (ComplexExpressionBoundNode.TryCreate(expression, parent, out var complexExpressionSymbol))
        {
            _childBoundNodes = [complexExpressionSymbol];
        }
        else
        {
            _childBoundNodes = [new DefaultExpressionBoundNode(expression, parent)];
        }
    }

    public bool IsChildExpressionComplex => _childBoundNodes switch
    {
        [ComplexExpressionBoundNode] => true,
        _ => false
    };

    public ArgumentListBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public IReadOnlyList<IExpressionBoundNode> ChildBoundNodes => _childBoundNodes;

    public ArgumentSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;
}
