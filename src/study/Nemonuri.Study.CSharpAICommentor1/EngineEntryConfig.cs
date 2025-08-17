namespace Nemonuri.Study.CSharpAICommentor1;

using C = Constants;

public class EngineEntryConfig() : IEngineEntryConfig
{
    public FileInfo? SourceFileInfo { get; set; }

    public string? SourceCode { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertSourceFilePathAndSourceCodeValid(IEngineEntryConfig ec)
    {
        Debug.Assert((ec.SourceFileInfo, ec.SourceCode) is ({ }, null) or (null, { }));
        Debug.Assert
        (
            !(ec.SourceFileInfo is { } sourceFileInfo) ||
            sourceFileInfo.Exists
        );
    }


    public Uri? OllamaServerUri { get; set; }

    public bool EnableRunLocalOllamaServer { get; set; }

    public string? OllamaLocalServerCommand { get; set; }

    //public IEnumerable<string>? OllamaLocalServerStartExtraArgumentList { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertOllamaServerConfigsValid(IEngineEntryConfig ec)
    {
        Debug.Assert(ec.OllamaServerUri is not null);
        Debug.Assert(!ec.EnableRunLocalOllamaServer || ec.OllamaLocalServerCommand is not null);
    }


    public string? TemplateEngineCommand { get; set; }

    public IEnumerable<string>? TemplateEngineArguments { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertTemplateEngineConfigsAreValid(IEngineEntryConfig ec)
    {
        //Debug.Assert(ec.TemplateEngineCommand is not null);
    }


    public bool EnableLog { get; set; }

    public DirectoryInfo? LogDirectoryInfo { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertLogConfigsAreValid(IEngineEntryConfig ec)
    {
        Debug.Assert
        (
            !ec.EnableLog ||
                (ec.LogDirectoryInfo is { } logDirectoryInfo) &&
                logDirectoryInfo.Exists
        );
    }


    [Conditional(C.Debug)]
    internal static void AssertValid(IEngineEntryConfig ec)
    {
        Debug.Assert(ec is not null);
        AssertSourceFilePathAndSourceCodeValid(ec);
        AssertOllamaServerConfigsValid(ec);
        AssertTemplateEngineConfigsAreValid(ec);
        AssertLogConfigsAreValid(ec);
    }
}
