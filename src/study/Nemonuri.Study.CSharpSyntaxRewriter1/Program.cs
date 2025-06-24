using Nemonuri.Study.CSharpSyntaxRewriter1;

#if DEBUG
//--- 디버그용 커맨드 입력 ---
{
    Console.WriteLine("Write Command:");
    var readLine = Console.ReadLine();
    args = readLine is { } v1 ? System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(v1).ToArray() : [];
}
//---|
#endif

if (CommandParsingTheory.Parse(args) is not { } parseResult) { return; }

SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(parseResult.TargetFile.FullName));
CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

var datas = CSharpSyntaxTransformTheory.GetExpressionInArgumentInBlockStructureRawDatas(root);

{
    if
    (
        datas[0] is { ExpressionSyntax: { } e } and { ArgumentSyntax: { } a } and { BlockSyntax: { } b }
    )
    {
        var newRoot = root.ReplaceNode(b, CSharpSyntaxTransformTheory.TransformToLocalDeclare(e, a, b));
        Console.WriteLine(newRoot.NormalizeWhitespace().ToFullString());
    }
}