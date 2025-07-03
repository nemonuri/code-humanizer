namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static partial class CSharpSyntaxRewritingTheory
{
    private static volatile BinderFactory? _binderFactory = null;

    private static BinderFactory BinderFactory => _binderFactory ?? new BinderFactory
    (
        static node => node switch
        {
            CompilationUnitSyntax or ArgumentSyntax => static n => n is BlockSyntax,
            BlockSyntax => static n => n is ArgumentListSyntax,
            ArgumentListSyntax => static n => n is ArgumentSyntax,
            _ => throw new InvalidOperationException($"Invalid syntax node type. {node.GetType()}")
        }
    );

    public static CSharpSyntaxTree ConvertComplexArgumentExpressionsToLocalDeclareStatements(CSharpSyntaxTree tree)
    {
        BoundNode boundTree = BinderFactory.GetBinder(tree.GetCompilationUnitRoot(), null).Bind();
        throw new NotImplementedException();
    }

}
