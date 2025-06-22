using Nemonuri.Study.CSharpSyntaxRewriter1;

if (CommandParsingTheory.Parse(args) is not { } parseResult) { return; }

SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(parseResult.TargetFile.FullName));
CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

//CSharpSyntaxVisitor