namespace Nemonuri.Study.CSharpSyntaxRewriter2;

using BoundNodes;

public static class BoundNodeWalkTheory
{
    public static void Walk<TWalkContext>(this IBoundNode boundNode, TWalkContext walkContext, ReadOnlySpan<int> pausedAddress = default)
        where TWalkContext : IWalkContext
    {
        WalkState walkState = WalkState.None;
        boundNode.WalkCore(walkContext, pausedAddress, default, ref walkState);
    }

    internal static void WalkCore<TWalkContext>
    (
        this IBoundNode boundNode,
        TWalkContext walkContext,
        ReadOnlySpan<int> pausedAddress,
        ReadOnlySpan<int> currentAddress,
        ref WalkState walkState
    )
        where TWalkContext : IWalkContext
    {
        if (boundNode is IChildBoundNodeProvider { ChildBoundNodes: { } childBoundNodes })
        {
            int pl = pausedAddress.Length;
            int cl = currentAddress.Length;

            Span<int> childAddress = stackalloc int[cl + 1];
            if (pl >= cl + 1)
            {
                pausedAddress[..cl].CopyTo(childAddress);
                childAddress[^1] = pausedAddress[cl + 1] + 1;
            }
            else
            {
                currentAddress.CopyTo(childAddress);
                childAddress[^1] = 0;
            }

            for (int i = childAddress[^1]; i < childBoundNodes.Count; i++)
            {
                IBoundNode child = childBoundNodes[i];
                child.WalkCore(walkContext, pausedAddress, childAddress, ref walkState);
                if (walkState == WalkState.Pause) { return; }
                childAddress[^1]++;
            }
        }

        walkContext.OnWalked(boundNode, currentAddress);
        walkState = walkContext.GetRequiredState(boundNode, currentAddress);
        if (walkState == WalkState.Pause)
        { 
            walkContext.OnPaused(boundNode, currentAddress);
        }
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

    public static bool TryFindDescendantByRelativeAdress(this IBoundNode boundNode, ReadOnlySpan<int> relativeAddress, [NotNullWhen(true)] out IBoundNode? found)
    {
        if (relativeAddress.IsEmpty)
        {
            found = boundNode;
            return true;
        }

        if (!(boundNode is IChildBoundNodeProvider { ChildBoundNodes: { } childBoundNodes }))
        {
            found = null;
            return false;
        }

        int index = relativeAddress[0];
        if (childBoundNodes.Count <= index)
        {
            found = null;
            return false;
        }

        return childBoundNodes[index].TryFindDescendantByRelativeAdress(relativeAddress[1..], out found);
    }
}