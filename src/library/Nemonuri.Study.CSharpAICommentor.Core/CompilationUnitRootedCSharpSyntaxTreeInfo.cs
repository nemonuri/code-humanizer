using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nemonuri.Study.CSharpAICommentor;

public record class CompilationUnitRootedCSharpSyntaxTreeInfo
{
    public CompilationUnitRootedCSharpSyntaxTreeInfo(CSharpSyntaxTree tree, CompilationUnitSyntax root, bool isMissing)
    {
        Tree = tree;
        Root = root;
        IsMissing = isMissing;
    }

    public CSharpSyntaxTree Tree { get; }
    public CompilationUnitSyntax Root { get; }
    public bool IsMissing { get; }
}