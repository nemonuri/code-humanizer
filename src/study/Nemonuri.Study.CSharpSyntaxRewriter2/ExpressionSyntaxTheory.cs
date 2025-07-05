// original source: 
// https://github.com/dotnet/roslyn/blob/main/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Extensions/ExpressionSyntaxExtensions.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static class ExpressionSyntaxTheory
{
    public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenExpression)
            expression = parenExpression.Expression;

        return expression;
    }
}
