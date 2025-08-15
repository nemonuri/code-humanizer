using System.Diagnostics.CodeAnalysis;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Net;

namespace Nemonuri.Study.CSharpAICommentor1;

public class CommandLinePremise
{
    public Argument<FileInfo> SourceFileInfo { get; } =
        new Argument<FileInfo>(nameof(SourceFileInfo))
        { 
            Description = "Path of a source code file"
        }.AcceptExistingOnly();

    public Option<Uri> OllamaServerUri { get; } =
        new Option<Uri>("--server")
        {
            Description = "Absolute URI or port number(loopback) of Ollama server.",
            Arity = ArgumentArity.ExactlyOne,
            DefaultValueFactory =
                static argumentResult => new UriBuilder() { Port = 11434}.Uri,
            CustomParser = static argumentResult =>
            {
                string rawString = argumentResult.Tokens[0].Value;
                if (Uri.TryCreate(rawString, UriKind.Absolute, out var uri))
                {
                    return uri;
                }
                else if (int.TryParse(rawString, out int port))
                {
                    if (!(port is (>= IPEndPoint.MinPort) and (<= IPEndPoint.MaxPort)))
                    {
                        argumentResult.AddError($"port number is out of range. {port} is less than -1 or greater than 65,535.");
                        return default;
                    }

                    return new UriBuilder() { Port = port }.Uri;
                }

                argumentResult.AddError($"{rawString} is not valid URI or port number.");
                return default;
            }
        };

    public RootCommand RootCommand { get; }

    private EntryConfig? _entryConfig;

    public CommandLinePremise()
    {
        RootCommand = new("Nemonuri.Study.CSharpAICommentor1")
        {
            SourceFileInfo,
            OllamaServerUri
        };

        RootCommand.SetAction
        (
            parseResult =>
            {
                EntryConfig config = new();

                if (parseResult.GetValue(SourceFileInfo) is not { } sourceFileInfo)
                { return 1; }
                config.SourceFileInfo = sourceFileInfo;

                if (parseResult.GetValue(OllamaServerUri) is not { } ollamaServerUri)
                { return 1; }
                config.OllamaServerUri = ollamaServerUri;

                return 0;
            }
        );
    }

    public bool TryParse
    (
        string[] args,
        [NotNullWhen(true)] out EntryConfig? entryConfig,
        out int exitCode
    )
    {
        Guard.IsNotNull(args);

        _entryConfig = default;
        exitCode = RootCommand.Parse(args).Invoke();
        entryConfig = _entryConfig;
        bool result = entryConfig is not null;
        Debug.Assert(!result || exitCode == 0);

        return result;
    }

    [Conditional(Constants.Debug)]
    public void ReadAndReplaceArgsIfDebuggerAttached(ref string[] args)
    {
        if (!Debugger.IsAttached) { return; }

        Debug.WriteLine("Enter new arguments:");
        string? rawInput = Console.ReadLine();
        if (rawInput is null) { return; }

        args = System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(rawInput).ToArray();
    }
}