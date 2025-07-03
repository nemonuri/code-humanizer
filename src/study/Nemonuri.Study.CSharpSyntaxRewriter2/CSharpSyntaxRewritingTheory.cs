using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static partial class CSharpSyntaxRewritingTheory
{
    private static volatile BinderFactory? _binderFactory = null;

    private static BinderFactory BinderFactory => _binderFactory ??= new BinderFactory
    (
        static node => node switch
        {
            CompilationUnitSyntax or ArgumentSyntax => static n => n is BlockSyntax,
            BlockSyntax => static n => n is StatementSyntax,
            StatementSyntax => static n => n is ArgumentListSyntax,
            ArgumentListSyntax => static n => n is ArgumentSyntax,
            _ => throw new InvalidOperationException($"Invalid syntax node type. {node.GetType()}")
        }
    );

    public static CSharpSyntaxTree ConvertComplexArgumentExpressionsToLocalDeclareStatements(CSharpSyntaxTree tree)
    {
        BoundNode boundTree = BinderFactory.CreateOrGetBinder(tree.GetCompilationUnitRoot(), null).Bind();

        int vNumber = 0;
        BoundNodeFindingWalkContext WalkContext = new(static (boundNode, _) => boundNode.Node.IsKind(SyntaxKind.Argument) && boundNode.Children.IsEmpty);
        boundTree.Walk(BinderFactory, WalkContext, WalkContext.PausedAddress.AsSpan());

        if
        (
            WalkContext.PausedBoundNode is { Node: ArgumentSyntax argument } bound &&
            WalkContext.PausedBinderFactory is { } factory &&
            factory.BinderMap.TryGetValue(argument, out Binder? binder) &&
            argument.Expression.WalkDownParentheses() is var originalExpression &&
            originalExpression is not (IdentifierNameSyntax or LiteralExpressionSyntax)
        )
        {
            string vName = "v" + (++vNumber);
            var newArgument = argument.WithExpression(IdentifierName(vName));

            //--- replace bound node ---
            var newBinder = factory.CreateOrGetBinder(newArgument, binder.Next);
            var newBound = newBinder.Bind();
            //---|

        }

    }

}
