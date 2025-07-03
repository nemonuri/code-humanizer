namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public class Binder
{
    public BinderFactory Factory { get; }
    public SyntaxNode Node { get; }
    public Binder? Next { get; }
    
    public SyntaxNodePredicate ChildPredicate { get; }

    internal Binder
    (
        BinderFactory factory,
        SyntaxNode node,
        Binder? next,
        SyntaxNodePredicate childPredicate
    )
    {
        Factory = factory;
        Node = node;
        Next = next;
        ChildPredicate = childPredicate;
    }

    public BoundNode Bind()
    {
        var childSyntaxes = Node.GetDescendantNodes(ChildPredicate);
        var childBinders = childSyntaxes.Select(n => Factory.GetBinder(n, this));
        return new BoundNode(Node, childBinders.Select(static b => b.Bind()));
    }
}

public delegate bool SyntaxNodePredicate(SyntaxNode node);

public delegate SyntaxNodePredicate SyntaxNodePredicateFactory(SyntaxNode node);