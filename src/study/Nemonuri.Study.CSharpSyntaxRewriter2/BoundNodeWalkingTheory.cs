namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static class BoundNodeWalkTheory
{
    public static void Walk(this BoundNode boundNode, BinderFactory binderFactory, IWalkContext walkContext, ReadOnlySpan<int> pausedAddress = default)
    {
        WalkState walkState = WalkState.None;
        boundNode.WalkCore(binderFactory, walkContext, pausedAddress, default, ref walkState);
    }

    internal static void WalkCore
    (
        this BoundNode boundNode,
        BinderFactory binderFactory,
        IWalkContext walkContext,
        ReadOnlySpan<int> pausedAddress,
        ReadOnlySpan<int> currentAddress,
        ref WalkState walkState
    )
    {
        if (CompareAddress(pausedAddress, currentAddress) >= 0)
        {
            walkContext.OnWalking(boundNode, binderFactory, currentAddress);
        }

        Span<int> childAddress = (pausedAddress.Length, currentAddress.Length) switch
        {
            var (l, r) when l >= r + 1 => [.. pausedAddress[..r], (pausedAddress[r + 1] + 1)],
            _ => [.. currentAddress, 0]
        };

        for (int i = childAddress[^1]; i < boundNode.Children.Length; i++)
        {
            BoundNode? child = boundNode.Children[i];
            child.WalkCore(binderFactory, walkContext, pausedAddress, childAddress, ref walkState);
            if (walkState == WalkState.Pause) { return; }
            childAddress[^1]++;
        }

        walkState = walkContext.OnWalked(boundNode, binderFactory, currentAddress);
    }

    private static int CompareAddress(ReadOnlySpan<int> lhs, ReadOnlySpan<int> rhs)
    {
        int minLength = Math.Min(lhs.Length, rhs.Length);

        for (int i = 0; i < minLength; i++)
        {
            int compareResult = lhs[i].CompareTo(rhs[i]);
            if (compareResult != 0) { return compareResult; }
        }

        return lhs.Length.CompareTo(rhs.Length);
    }

    public static bool TryFindDescendantByRelativeAdress(this BoundNode boundNode, ReadOnlySpan<int> relativeAddress, [NotNullWhen(true)] out BoundNode? found)
    {
        if (relativeAddress.IsEmpty)
        {
            found = boundNode;
            return true;
        }

        int index = relativeAddress[0];
        if (boundNode.Children.Length <= index)
        {
            found = null;
            return false;
        }

        return boundNode.Children[index].TryFindDescendantByRelativeAdress(relativeAddress[1..], out found);
    }
}