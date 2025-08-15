namespace Nemonuri.Study.CSharpAICommentor1;

using C = Constants;

public class EntryConfig()
{
    public FileInfo? SourceFileInfo { get; set; }

    public string? SourceCode { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertSourceFilePathAndSourceCodeValid(EntryConfig ec)
    {
        Debug.Assert((ec.SourceFileInfo, ec.SourceCode) is ({ }, null) or (null, { }));
        Debug.Assert
        (
            !(ec.SourceFileInfo is { } sourceFileInfo) ||
            sourceFileInfo.Exists
        );
    }


    public Uri? OllamaServerUri { get; set; }

    public string? OllamaLocalServerStartCommand { get; set; }

    public IEnumerable<string>? OllamaLocalServerStartExtraArgumentList { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertOllamaServerConfigsValid(EntryConfig ec)
    {
        Debug.Assert(ec.OllamaServerUri is not null);
    }


    public string? TemplateEngineStartCommand { get; set; }

    public IEnumerable<string>? TemplateEngineStartExtraArgumentList { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertTemplateEngineConfigsAreValid(EntryConfig ec)
    {
        Debug.Assert(ec.TemplateEngineStartCommand is not null);
    }


    public bool EnableLog { get; set; }

    public DirectoryInfo? LogDirectoryInfo { get; set; }

    [Conditional(C.Debug)]
    internal static void AssertLogConfigsAreValid(EntryConfig ec)
    {
        Debug.Assert
        (
            !ec.EnableLog ||
                (ec.LogDirectoryInfo is { } logDirectoryInfo) &&
                logDirectoryInfo.Exists
        );
    }


    [Conditional(C.Debug)]
    internal static void AssertValid(EntryConfig ec)
    {
        Debug.Assert(ec is not null);
        AssertSourceFilePathAndSourceCodeValid(ec);
        AssertOllamaServerConfigsValid(ec);
        AssertTemplateEngineConfigsAreValid(ec);
        AssertLogConfigsAreValid(ec);
    }
}
