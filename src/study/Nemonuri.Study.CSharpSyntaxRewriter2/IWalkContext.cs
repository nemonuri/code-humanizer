namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public interface IWalkContext
{
    void OnWalking(BoundNode boundNode, BinderFactory binderFactory, ReadOnlySpan<int> address);
    WalkedResult OnWalked(BoundNode boundNode, BinderFactory binderFactory, ReadOnlySpan<int> address);
}

public enum WalkedResult
{
    None = 0,
    Pause = 1
}
