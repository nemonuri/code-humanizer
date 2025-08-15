
using Nemonuri.Study.CSharpAICommentor1;

// 
CommandLinePremise commandLinePremise = new();
commandLinePremise.ReadAndReplaceArgsIfDebuggerAttached(ref args);
if (!commandLinePremise.TryParse(args, out var entryConfig, out var exitCode))
{ return exitCode; }

StatusForTest.AssertConfigured();

// 

StatusForTest.AssertOriginalSyntaxTreeIsValid();

//

StatusForTest.AssertRewritedSyntaxTreeIsValid();

//

StatusForTest.AssertCommentTriviaAddedTreeIsValid();

//

StatusForTest.AssertAICommentedCodeIsValid();

return 0;