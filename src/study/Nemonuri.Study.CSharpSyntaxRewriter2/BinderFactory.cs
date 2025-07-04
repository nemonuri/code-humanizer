
namespace Nemonuri.Study.CSharpSyntaxRewriter2;

#if false
public class BinderFactory
{
    internal readonly Dictionary<SyntaxNode, Binder> _binderCacheMap = new();

    private readonly BinderFactoryVisitor _binderFactoryVisitor;

    public IReadOnlyDictionary<SyntaxNode, Binder> BinderCacheMap => _binderCacheMap;

    public BinderFactory()
    {
        _binderFactoryVisitor = new(this);
    }

    public SyntaxNodePredicateFactory ChildPredicateFactory { get; }

    public BinderFactory(SyntaxNodePredicateFactory childPredicateFactory)
    {
        ChildPredicateFactory = childPredicateFactory;
        _binderFactoryVisitor = new(this);
    }

    public Binder CreateOrGetBinder
    (
        SyntaxNode syntax,
        Binder? nextBinder
    )
    {
        if (!_binderCacheMap.TryGetValue(syntax, out Binder? binder))
        {
            binder = new Binder(this, syntax, nextBinder, ChildPredicateFactory(syntax));
            _binderCacheMap.Add(syntax, binder);
        }
        return binder;
    }

    public bool RemoveBinder(Binder binder)
    {
        return _binderCacheMap.Remove(binder.Node);
    }


    public Binder? GetBinder(SyntaxNode node)
    {
        return _binderFactoryVisitor.Visit(node);
    }
}
#endif