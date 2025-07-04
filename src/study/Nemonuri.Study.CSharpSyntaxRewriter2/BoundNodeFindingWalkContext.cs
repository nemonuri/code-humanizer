#if false
namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public class BoundNodeFindingWalkContext : IWalkContext
{
    public BoundNodeFindingWalkContext(Func<BoundNode, BinderFactory, bool> predicate)
    {
        Predicate = predicate;
    }

    public Func<BoundNode, BinderFactory, bool> Predicate { get; }

    public BoundNode? PausedBoundNode { get; private set; }

    public BinderFactory? PausedBinderFactory { get; private set; }

    public ImmutableArray<int> PausedAddress { get; private set; } = [];

    public void OnWalking(BoundNode boundNode, BinderFactory binderFactory, ReadOnlySpan<int> address)
    {
    }

    public WalkState OnWalked(BoundNode boundNode, BinderFactory binderFactory, ReadOnlySpan<int> address)
    {
        if (Predicate(boundNode, binderFactory))
        {
            PausedBoundNode = boundNode;
            PausedBinderFactory = binderFactory;
            PausedAddress = address.ToImmutableArray();
            return WalkState.Pause;
        }
        else
        {
            PausedBoundNode = null;
            PausedBinderFactory = null;
            PausedAddress = [];
            return WalkState.None;
        }
    }
}
#endif