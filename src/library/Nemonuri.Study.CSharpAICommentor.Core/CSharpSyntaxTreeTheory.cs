
using Microsoft.CodeAnalysis.CSharp;

namespace Nemonuri.Study.CSharpAICommentor;

using Logging;

public static partial class CSharpSyntaxTreeTheory
{
    public static async Task<CreateCSharpSyntaxTreeFromFileResult>
    CreateCSharpSyntaxTreeFromFileAsync
    (
        FileInfo fileInfo,
        ILogger? logger = null,
        CancellationToken cancellationToken = default
    )
    {
        Guard.IsNotNull(fileInfo);
        logger ??= NullLogger.Instance;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogMessageAndMemberWithCaller("Run File.ReadAllTextAsync", fileInfo);

            string text;
            try
            {
                text = await File.ReadAllTextAsync(fileInfo.FullName, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (IsReadAllTextException(e))
            {
                return CreateCSharpSyntaxTreeFromFileResult.CreateAsReadAllTextFailed(e, e.Message);
            }

            logger.LogMessageAndMemberWithCaller("Invoke CSharpSyntaxTree.ParseText", text);
            var parsed = CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);
            return CreateCSharpSyntaxTreeFromFileResult.CreateAsValue((CSharpSyntaxTree)parsed);
        }
        catch (OperationCanceledException e)
        {
            return CreateCSharpSyntaxTreeFromFileResult.CreateAsCanceled(e.Message);
        }
    }

    internal static bool IsReadAllTextException(Exception e)
    {
        return e is
            ArgumentException or
            ArgumentNullException or
            PathTooLongException or
            DirectoryNotFoundException or
            IOException or
            UnauthorizedAccessException or
            FileNotFoundException or
            NotSupportedException or
            System.Security.SecurityException
            ;
    }
}
