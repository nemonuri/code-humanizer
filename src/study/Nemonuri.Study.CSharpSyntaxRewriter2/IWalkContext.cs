namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public interface IWalkContext
{
    void OnWalking(BoundNode boundNode, BinderFactory binderFactory, ReadOnlySpan<int> address);
    WalkState OnWalked(BoundNode boundNode, BinderFactory binderFactory, ReadOnlySpan<int> address);
}

public enum WalkState
{
    None = 0,
    Pause = 1
}
