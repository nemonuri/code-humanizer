
using System.Diagnostics;
using Nemonuri.Study.CSharpSyntaxRewriter3;
using Nemonuri.Trees;
using Nemonuri.Trees.CodeAnalysis.CSharp;
using Nemonuri.Trees.Indexes;
using Nemonuri.Trees.RoseNodes;

#if DEBUG
//--- 디버그용 커맨드 입력 ---
{
    Console.WriteLine("Write Command:");
    var readLine = Console.ReadLine();
    args = readLine is { } v1 ? System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(v1).ToArray() : [];
}
//---|
#endif

if (CommandParsingTheory.Parse(args) is not { } parseResult) { return 1; }

SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(parseResult.TargetFile.FullName));

SyntaxNodeOrTokenChildProvider childProvider = new();
AdHocIndexedPathAggregatingPremise<SyntaxNodeOrToken, ImmutableArray<RoseNode<SyntaxNodeInfo>>> aggregatingPremise = new
(
    defaultSeedProvider: static () => [],
    optionalAggregator: (siblingsSeed, childrenSeed, source) =>
    {
        if (!source.IndexedPath.TryGetLastNode(out var syntaxNodeOrToken))
        { return (default, false); }

        if (syntaxNodeOrToken.AsNode() is ArgumentSyntax argument)
        {
            if (!(argument.Expression is IdentifierNameSyntax or LiteralExpressionSyntax))
            {
                RoseNode<SyntaxNodeInfo> newNode = new(new(argument, source.IndexedPath.ToIndexSequence()), childrenSeed);
                return ([.. siblingsSeed, newNode], true);
            }
        }
        else if (syntaxNodeOrToken.AsNode() is BlockSyntax block)
        {
            if (childrenSeed.Length > 0)
            {
                RoseNode<SyntaxNodeInfo> newNode = new(new(block, source.IndexedPath.ToIndexSequence()), childrenSeed);
                return ([.. siblingsSeed, newNode], true);
            }
        }
        else if (syntaxNodeOrToken.AsNode() is CompilationUnitSyntax compilationUnit)
        { 
            RoseNode<SyntaxNodeInfo> newNode = new(new(compilationUnit, source.IndexedPath.ToIndexSequence()), childrenSeed);
            return ([.. siblingsSeed, newNode], true);
        }

        return ([.. siblingsSeed, .. childrenSeed], true);
    }
);

if (WalkingTheory.TryWalkAsRoot(aggregatingPremise, childProvider, tree.GetRoot(), out var walkedValue))
{
    Debug.Assert(walkedValue.Length == 1);
    // TODO
    return 0;
}
else
{
    return 1;
}