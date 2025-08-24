
using System.Runtime.CompilerServices;

namespace Nemonuri.Study.CSharpAICommentor.Logging;

public static partial class LogTheory
{
    [LoggerMessage(
        Message = "[{CallerMemberName}] {Message}"
    )]
    public static partial void LogMessageWithCaller
    (this ILogger logger, string message, LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string callerMemberName = "");

    [LoggerMessage(
        Message = "[{CallerMemberName}] {Message} "
            + "{Arg0Expression} = {Arg0}"
    )]
    public static partial void LogMessageAndMemberWithCaller
    (
        this ILogger logger,
        string message,
        object? arg0,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string callerMemberName = "",
        [CallerArgumentExpression(nameof(arg0))] string arg0Expression = ""
    );

    [LoggerMessage(
        Message = "[{CallerMemberName}] {Message} "
            + "{Arg0Expression} = {Arg0}, {Arg1Expression} = {Arg1}"
    )]
    public static partial void LogMessageAndMemberWithCaller
    (
        this ILogger logger,
        string message,
        object? arg0,
        object? arg1,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string callerMemberName = "",
        [CallerArgumentExpression(nameof(arg0))] string arg0Expression = "",
        [CallerArgumentExpression(nameof(arg1))] string arg1Expression = ""
    );
}