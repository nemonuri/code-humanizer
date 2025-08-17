namespace Nemonuri.Study.CSharpAICommentor1;

public interface IEngineEntryConfig
{
    FileInfo? SourceFileInfo { get; }
    string? SourceCode { get; }
    Uri? OllamaServerUri { get; }
    bool EnableRunLocalOllamaServer { get; }
    string? OllamaLocalServerCommand { get; }
    string? TemplateEngineCommand { get; }
    IEnumerable<string>? TemplateEngineArguments { get; }
    bool EnableLog { get; }
    DirectoryInfo? LogDirectoryInfo { get; }
}