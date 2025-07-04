using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nemonuri.Study.CSharpSyntaxRewriter2;

using BoundNodes;

public static partial class CSharpSyntaxRewritingTheory
{
    public static CSharpSyntaxTree ConvertComplexArgumentExpressionsToLocalDeclareStatements(CSharpSyntaxTree tree)
    {
        PauseStateProvidingWalkContext walkContext = new PauseStateProvidingWalkContext
        (
            new RawWalkContext()
            {
                GetRequiredStateDelegate = static (b, _) =>
                    b is ArgumentBoundNode argumentBoundNode &&
                    argumentBoundNode.Syntax.Expression is var e &&
                    e.WalkDownParentheses() != e ?
                        WalkState.Pause : WalkState.None
            }
        );
        CompilationUnitBoundNode boundTree = new(tree.GetCompilationUnitRoot());

        do
        {
            boundTree.Walk(walkContext, walkContext.Address);
            if (walkContext.WalkState == WalkState.Pause)
            {

            }
        }
        while (walkContext.Address.IsEmpty);

#if false
        BoundNode boundTree = BinderFactory.CreateOrGetBinder(tree.GetCompilationUnitRoot(), null).Bind();

        int vNumber = 0;
        BoundNodeFindingWalkContext WalkContext = new(static (boundNode, _) => boundNode.Syntax.IsKind(SyntaxKind.Argument) && boundNode.Children.IsEmpty);
        boundTree.Walk(BinderFactory, WalkContext, WalkContext.PausedAddress.AsSpan());

        if
        (
            WalkContext.PausedBoundNode is { Syntax: ArgumentSyntax argument } bound &&
            WalkContext.PausedBinderFactory is { } factory &&
            factory.BinderCacheMap.TryGetValue(argument, out Binder? binder) &&
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
#endif

        throw new NotImplementedException();
    }


}
