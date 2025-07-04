
namespace Nemonuri.Study.CSharpSyntaxRewriter2;

internal class BinderFactoryVisitor : CSharpSyntaxVisitor<Binder>
{
    public BinderFactory BinderFactory { get; }

    public BinderFactoryVisitor(BinderFactory binderFactory) : base()
    {
        BinderFactory = binderFactory;
    }

    public override Binder? DefaultVisit(SyntaxNode node)
    {
        return Visit(node.Parent);
    }

    public override Binder? VisitCompilationUnit(CompilationUnitSyntax node)
    {
        if (!BinderFactory.BinderCacheMap.TryGetValue(node, out var result))
        {
            result = new Binder(BinderFactory);
            BinderFactory._binderCacheMap.TryAdd(node, result);
        }

        return result;
    }
}