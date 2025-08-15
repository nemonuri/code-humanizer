namespace Nemonuri.Study.CSharpAICommentor1;

internal static class Constants
{
    public const string E2eTest = "E2E_TEST";
}

internal class StatusModel
{
    private readonly static StatusModel s_instance = new();

    private StatusModel() { }

    public Func<CSharpSyntaxTree>? OriginalSyntaxTreeProvider { get; set; }

    public CSharpSyntaxTree? OriginalSyntaxTree { get; set; }

    [Conditional(Constants.E2eTest)]
    public static void Update(Action<StatusModel> updator)
    {
        updator(s_instance);
    }

    [Conditional(Constants.E2eTest)]
    public static void AssertConfiured()
    {
        Debug.Assert(s_instance.OriginalSyntaxTreeProvider is not null);

    }

    [Conditional(Constants.E2eTest)]
    public static void AssertOriginalSyntaxTreeIsValid()
    {
        Debug.Assert(s_instance.OriginalSyntaxTree is not null);
        Debug.Assert(s_instance.OriginalSyntaxTree.HasCompilationUnitRoot);
        Debug.Assert(!s_instance.OriginalSyntaxTree.GetRoot().IsMissing);

        Debug.Assert(s_instance.OriginalSyntaxTreeProvider is not null);
        Debug.Assert(s_instance.OriginalSyntaxTreeProvider.Invoke().IsEquivalentTo(s_instance.OriginalSyntaxTree));
    }

    [Conditional(Constants.E2eTest)]
    public static void Assertsdf()
    {
        
    }
}