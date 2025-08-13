
using System.Text;
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

if (!RewriteWalkingTheory.TryGetRewriteSourceInfoRoseNode(tree, out var rewriteSourceInfoRoseNode))
{
    return 1;
}

if (!RewriteWalkingTheory.TryGetSortedRewriteSourceInfos(rewriteSourceInfoRoseNode, out var sortedRewriteSourceInfos))
{ 
    return 1;
}

//WriteRewriteSourceInfosToConsole(sortedRewriteSourceInfos);

var newTree = RewriteSourceInfoTheory.CreateRewritedSyntaxTree(sortedRewriteSourceInfos);
Console.WriteLine(newTree.ToString());

//Console.WriteLine("---");
//WriteRewriteSourceInfosToConsole(sortedRewriteSourceInfos);

return 0;

/*
static void WriteRewriteSourceInfosToConsole(RewriteSourceInfo[] rewriteSourceInfos)
{
    Console.WriteLine
    (
        string.Join
        (
            Environment.NewLine,
            rewriteSourceInfos.Select
            (
                static a =>
                {
                    StringBuilder sb = new();
                    a.IndexSequence.PrintInternalList(sb);
                    sb.AppendLine();
                    sb.AppendLine(a.SyntaxNode.ToString()).Append("---");
                    return sb.ToString();
                }
            )
        )
    );
}
*/