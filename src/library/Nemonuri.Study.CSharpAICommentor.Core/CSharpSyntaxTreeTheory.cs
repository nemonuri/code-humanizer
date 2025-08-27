
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nemonuri.Failures;
using Nemonuri.Study.CSharpAICommentor.Logging;
using Rw = Nemonuri.Study.CSharpAICommentor.CSharpSyntaxTreeRewriters;

namespace Nemonuri.Study.CSharpAICommentor;

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

            logger.LogMessageWithCaller("Invoke CSharpSyntaxTree.ParseText");
            var parsed = Rw.CSharpSyntaxNodeTheory.CreateCSharpSyntaxTreeFromText(text, cancellationToken);
            return CreateCSharpSyntaxTreeFromFileResult.CreateAsValue(parsed);
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

    public static async Task<CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult>
    CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync
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

            CreateCSharpSyntaxTreeFromFileResult createCSharpSyntaxTreeFromFileResult =
                await CreateCSharpSyntaxTreeFromFileAsync(fileInfo, logger, cancellationToken).ConfigureAwait(false);
            if (createCSharpSyntaxTreeFromFileResult.IsFailure)
            {
                return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsCreateCSharpSyntaxTreeFromFileFailed
                (
                    createCSharpSyntaxTreeFromFileResult.GetFailInfo(),
                    createCSharpSyntaxTreeFromFileResult.GetMessage()
                );
            }

            CSharpSyntaxTree csharpSyntaxTree = createCSharpSyntaxTreeFromFileResult.GetValue();

            logger.LogMessageWithCaller($"Run CSharpSyntaxTree.GetRootAsync");

            CSharpSyntaxNode root;
            try
            {
                root = await csharpSyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsGetRootFailed(e.Message);
            }

            if (root is not CompilationUnitSyntax compilationUnitSyntax)
            {
                return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsRootIsNotCompilationUnitSyntax(root.GetType());
            }
            logger.LogMessageWithCaller($"Confirm root is CompilationUnitSyntax.");

            bool isMissing = Rw.CSharpSyntaxNodeTheory.ContainsMissingNodeOrToken(compilationUnitSyntax);
            logger.LogMessageAndMemberWithCaller($"{nameof(Rw.CSharpSyntaxNodeTheory.ContainsMissingNodeOrToken)} invoked.", isMissing);

            return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsValue
            (
                new CompilationUnitRootedCSharpSyntaxTreeInfo
                (
                    csharpSyntaxTree,
                    compilationUnitSyntax,
                    isMissing
                )
            );
        }
        catch (OperationCanceledException e)
        {
            return CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.CreateAsCanceled(e.Message);
        }
    }
}
