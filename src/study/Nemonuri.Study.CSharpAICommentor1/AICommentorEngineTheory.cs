using C = Nemonuri.Study.CSharpAICommentor1.Constants;
using OllamaSharp;
using Nemonuri.OllamaRunning;
using Nemonuri.Study.CSharpAICommentor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nemonuri.Failures;

namespace Nemonuri.Study.CSharpAICommentor1;

public static partial class AICommentorEngineTheory
{
    public static async Task<RunResult> RunAsync
    (
        FileInfo fileInfo,

        Uri? serverUri = null,
        bool enableRunningLocalOllamaServer = false,
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,

        ILogger? logger = null,
        CancellationToken cancellationToken = default
    )
    {
        Guard.IsNotNull(fileInfo);
        logger ??= NullLogger.Instance;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result1 =
                await CSharpSyntaxTreeTheory.CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync
                (fileInfo, logger, cancellationToken).ConfigureAwait(false);
            if (result1.IsFailure)
            {
                return RunResult.CreateAsCreateCompilationUnitRootedCSharpSyntaxTreeInfoFailed
                (result1.GetFailInfo(), result1.GetMessage());
            }

            var treeInfo = result1.GetValue();
            if (treeInfo.IsMissing)
            {
                return RunResult.CreateAsIsMissing();
            }

            // 음...이건 '재사용' 해야 하지 않나?
            // 일단, '한 번' 이라도 제대로 돌아가는 걸 구현하자!
            var result2 = await OllamaRunningTheory.GetClientAfterEnsuringOllamaServerRunningAsync
            (
                serverUri, enableRunningLocalOllamaServer, localOllamaHostCommand,
                cancellationToken
            ).ConfigureAwait(false);
            if (result2.IsFailure)
            {
                return RunResult.CreateAsGetClientAfterEnsuringOllamaServerRunningFailed
                (result2.GetFailInfo(), result2.GetMessage());
            }

            // Transfrom tree

            // Request ollama to 

            // Untransfrom tree
        }
        catch (OperationCanceledException e)
        {
            return RunResult.CreateAsCanceled(e.Message);
        }

    }
}

