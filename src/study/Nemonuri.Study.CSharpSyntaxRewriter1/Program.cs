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

{
    var v1 = new CodeRewriter().Run(root);
    var v2 = new TriviaAdder().Visit(v1);
    if (v2 is { } newRoot)
    {
        Console.WriteLine(newRoot.NormalizeWhitespace().ToFullString());
    }
}


