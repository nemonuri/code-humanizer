
using Nemonuri.Study.CSharpSyntaxRewriter3;


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

if (RewriteWalkingTheory.TryGetRewriteSourceInfoRoseNode(tree, out var rewriteSourceInfoRoseNode))
{
    return 0;
}
else
{
    return 1;
}