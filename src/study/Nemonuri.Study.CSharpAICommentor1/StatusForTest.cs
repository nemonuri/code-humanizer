namespace Nemonuri.Study.CSharpAICommentor1;

using C = Constants;

internal class StatusForTest
{
    private readonly static StatusForTest s_instance = new();

    private StatusForTest() { }

    public Func<CSharpSyntaxTree?>? OriginalSyntaxTreeProvider { get; set; }

    public CSharpSyntaxTree? OriginalSyntaxTree { get; set; }

    public CSharpSyntaxTree? RewritedSyntaxTree { get; set; }

    public CSharpSyntaxTree? CommentTriviaAddedTree { get; set; }

    public string? AICommentedCode { get; set; }

    [Conditional(C.E2eTest)]
    public static void Update(Action<StatusForTest> updator)
    {
        updator(s_instance);
    }

    [Conditional(C.E2eTest)]
    public static void AssertConfigured()
    {
        Debug.Assert(s_instance.OriginalSyntaxTreeProvider is not null);

    }

    [Conditional(C.E2eTest)]
    public static void AssertOriginalSyntaxTreeIsValid()
    {
        Debug.Assert(s_instance.OriginalSyntaxTree is not null);
        Debug.Assert(s_instance.OriginalSyntaxTree.HasCompilationUnitRoot);
        Debug.Assert(!SyntaxNodeTheory.ContainsMissingNodeOrToken(s_instance.OriginalSyntaxTree.GetRoot()));

        Debug.Assert(s_instance.OriginalSyntaxTreeProvider is not null);
        Debug.Assert(s_instance.OriginalSyntaxTreeProvider.Invoke()?.IsEquivalentTo(s_instance.OriginalSyntaxTree) ?? false);
    }

    [Conditional(C.E2eTest)]
    public static void AssertRewritedSyntaxTreeIsValid()
    {

    }

    [Conditional(C.E2eTest)]
    public static void AssertCommentTriviaAddedTreeIsValid()
    {

    }

    [Conditional(C.E2eTest)]
    public static void AssertAICommentedCodeIsValid()
    {

    }
}