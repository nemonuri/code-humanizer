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
        new Option<Uri>("--server-uri")
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

    public Option<bool> EnableRunLocalOllamaServer { get; } =
        new Option<bool>("--enable-run-local-server")
        {
            Description = "Enable run local Ollama server app, if needed.",
            DefaultValueFactory = static _ => false
        };

    public Option<string> OllamaLocalServerCommand { get; } =
        new Option<string>("--ollama-command")
        {
            Description = "Command for access ollama local server app.",
            DefaultValueFactory = static _ => "ollama"
        };

    public Option<string?> TemplateEngineCommand { get; } =
        new Option<string?>("--template-command")
        {
            Description = "Command for access custom template engine app."
        };

    public Option<string[]?> TemplateEngineArguments { get; } =
        new Option<string[]?>("--template-args")
        {
            Description = "Arguments for running custom template engine app."
        };

    public Option<bool> EnableLog { get; } =
        new Option<bool>("--enable-log")
        {
            Description = "Enable log.",
            DefaultValueFactory = static _ => false
        };

    public Option<DirectoryInfo> LogDirectoryInfo { get; } =
        new Option<DirectoryInfo>("--log-directory")
        {
            Description = "Directory for write log file. Default is current directory.",
            DefaultValueFactory = static _ => new DirectoryInfo(AppContext.BaseDirectory)
        }.AcceptExistingOnly();

    public RootCommand RootCommand { get; }

    private EngineEntryConfig? _entryConfig;

    public CommandLinePremise()
    {
        RootCommand = new("Nemonuri.Study.CSharpAICommentor1")
        {
            SourceFileInfo,
            OllamaServerUri,
            EnableRunLocalOllamaServer,
            OllamaLocalServerCommand,
            TemplateEngineCommand,
            TemplateEngineArguments,
            EnableLog,
            LogDirectoryInfo
        };

        RootCommand.SetAction
        (
            parseResult =>
            {
                EngineEntryConfig config = new();

                try
                {
                    config.SourceFileInfo = parseResult.GetRequiredValue(SourceFileInfo);
                    config.OllamaServerUri = parseResult.GetRequiredValue(OllamaServerUri);
                    config.EnableRunLocalOllamaServer = parseResult.GetValue(EnableRunLocalOllamaServer);
                    config.OllamaLocalServerCommand = parseResult.GetValue(OllamaLocalServerCommand);
                    config.TemplateEngineCommand = parseResult.GetValue(TemplateEngineCommand);
                    config.TemplateEngineArguments = parseResult.GetValue(TemplateEngineArguments);
                    config.EnableLog = parseResult.GetValue(EnableLog);
                    config.LogDirectoryInfo = parseResult.GetRequiredValue(LogDirectoryInfo);

                    _entryConfig = config;
                }
                catch (InvalidOperationException e)
                {
                    Console.Error.WriteLine(e.Message);
                    return 1;
                }

                return 0;
            }
        );
    }

    public bool TryParse
    (
        string[] args,
        [NotNullWhen(true)] out EngineEntryConfig? entryConfig,
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