
namespace Nemonuri.Study.CSharpAICommentor1;

using C = Constants;

public class AICommentorEngine : IEngineEntryConfig
{
    private readonly EngineEntryConfig _entryConfig;

    public AICommentorEngine(EngineEntryConfig entryConfig)
    {
        _entryConfig = entryConfig;
        AssertEngineEntryConfigValid();
    }

    public async Task<EngineRunResult> RunAsync(CancellationToken cancellationToken = default)
    {
        StatusForTest.Update(s => s.OriginalSyntaxTreeProvider = () => CreateCSharpSyntaxTreeAsync().Result);
        StatusForTest.AssertConfigured();

        if (cancellationToken.IsCancellationRequested) { return CreateErrorResult(); }

        var originalSyntaxTree = await CreateCSharpSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
        if (originalSyntaxTree is null)
        { return CreateErrorResult("Cannot create csharp syntax tree."); }

        var originalSyntaxTreeRoot = await originalSyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
        if (originalSyntaxTreeRoot is null)
        { return CreateErrorResult("Cannot get csharp syntax root."); }

        // TODO: Root 노드에, missing 노드가 포함되어 있는지 확인하는 메서드 구현

        StatusForTest.Update(s => s.OriginalSyntaxTree = originalSyntaxTree);
        StatusForTest.AssertOriginalSyntaxTreeIsValid();

        // TODO: more implmentaion

        return new EngineRunResult(default);

        EngineRunResult CreateErrorResult(string errorMessage = "")
        {
            return new(cancellationToken.IsCancellationRequested ? "Cancel requested." : errorMessage);
        }
    }

    internal async Task<CSharpSyntaxTree?> CreateCSharpSyntaxTreeAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) { return default; }

        string? ensuredSourceCode;
        switch (SourceFileInfo, SourceCode)
        {
            case (_, { }):
                ensuredSourceCode = SourceCode;
                break;
            case ({ }, null):
                ensuredSourceCode = await File.ReadAllTextAsync(SourceFileInfo.FullName).ConfigureAwait(false);
                break;
            default:
                return default;
        }

        if (cancellationToken.IsCancellationRequested) { return default; }

        return
            CSharpSyntaxTree.ParseText(ensuredSourceCode, null, "", null, cancellationToken) as CSharpSyntaxTree;
    }

    public FileInfo? SourceFileInfo => _entryConfig.SourceFileInfo;

    public string? SourceCode => _entryConfig.SourceCode;

    public Uri? OllamaServerUri => _entryConfig.OllamaServerUri;

    public bool EnableRunLocalOllamaServer => _entryConfig.EnableRunLocalOllamaServer;

    public string? OllamaLocalServerCommand => _entryConfig.OllamaLocalServerCommand;

    public string? TemplateEngineCommand => _entryConfig.TemplateEngineCommand;

    public IEnumerable<string>? TemplateEngineArguments => _entryConfig.TemplateEngineArguments;

    public bool EnableLog => _entryConfig.EnableLog;

    public DirectoryInfo? LogDirectoryInfo => _entryConfig.LogDirectoryInfo;

    [Conditional(C.Debug)]
    private void AssertEngineEntryConfigValid()
    {
        EngineEntryConfig.AssertValid(this);
    }
}
