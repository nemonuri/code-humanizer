using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Nemonuri.ControlFlowGraphs;

#if DEBUG
//--- 디버그용 커맨드 입력 ---
{
    if (args.Length == 0)
    { 
        Console.WriteLine("Write Command:");
        var readLine = Console.ReadLine();
        args = readLine is { } v1 ? System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(v1).ToArray() : [];
    }
}
//---|
#endif

if (CommandParsingTheory.Parse(args) is not { } parsedResult) { return; }

string code = File.ReadAllText(parsedResult.TargetFile.FullName);
SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
Compilation compilation = CSharpCompilation.Create
(
    assemblyName: null,
    syntaxTrees: [tree],
    references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]
);

ControlFlowGraph? cfg = ControlFlowGraph.Create(tree.GetRoot(), compilation.GetSemanticModel(tree));

Console.WriteLine(cfg);