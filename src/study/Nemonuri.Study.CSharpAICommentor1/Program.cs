
using Nemonuri.Study.CSharpAICommentor1;

// 
CommandLinePremise commandLinePremise = new();
commandLinePremise.ReadAndReplaceArgsIfDebuggerAttached(ref args);
if (!commandLinePremise.TryParse(args, out var entryConfig, out var exitCode))
{ return exitCode; }

CancellationTokenSource cs = new ();
Console.CancelKeyPress += CancelKeyPress_Handle;

AICommentorEngine engine = new(entryConfig);
EngineRunResult runResult = await engine.RunAsync(cs.Token).ConfigureAwait(false);


if (runResult.ErrorMessage is { } errorMessage)
{
    Console.Error.WriteLine(errorMessage);
    return 1;
}


StatusForTest.AssertRewritedSyntaxTreeIsValid();

//

StatusForTest.AssertCommentTriviaAddedTreeIsValid();

//

StatusForTest.AssertAICommentedCodeIsValid();

return 0;

void CancelKeyPress_Handle(object? sender, ConsoleCancelEventArgs e)
{
    if (e.Cancel) { return; }

    if (!cs.IsCancellationRequested) { cs.Cancel(); }
    Console.CancelKeyPress -= CancelKeyPress_Handle;
}