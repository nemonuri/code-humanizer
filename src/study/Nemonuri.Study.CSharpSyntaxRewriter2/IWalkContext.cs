namespace Nemonuri.Study.CSharpSyntaxRewriter2;

using BoundNodes;

public interface IWalkContext
{
    void OnWalked(IBoundNode boundNode, ReadOnlySpan<int> address);
    WalkState GetRequiredState(IBoundNode boundNode, ReadOnlySpan<int> address);
    void OnPaused(IBoundNode boundNode, ReadOnlySpan<int> address);
    void OnWalkEntering();
}

public enum WalkState
{
    None = 0,
    Pause = 1
}

public struct RawWalkContext : IWalkContext
{
    public Action<IBoundNode, ReadOnlySpan<int>>? OnWalkedDelegate;
    public Func<IBoundNode, ReadOnlySpan<int>, WalkState>? GetRequiredStateDelegate;
    public Action<IBoundNode, ReadOnlySpan<int>>? OnPausedDelegate;
    public Action? OnWalkEnteringDelegate;

    public readonly void OnWalked(IBoundNode boundNode, ReadOnlySpan<int> address)
    {
        OnWalkedDelegate?.Invoke(boundNode, address);
    }

    public readonly WalkState GetRequiredState(IBoundNode boundNode, ReadOnlySpan<int> address)
    {
        return GetRequiredStateDelegate?.Invoke(boundNode, address) ?? WalkState.None;
    }

    public readonly void OnPaused(IBoundNode boundNode, ReadOnlySpan<int> address)
    {
        OnPausedDelegate?.Invoke(boundNode, address);
    }

    public readonly void OnWalkEntering()
    {
        OnWalkEnteringDelegate?.Invoke();
    }
}

public class PauseStateProvidingWalkContext : IWalkContext
{
    private IBoundNode? _boundNode = null;
    private int[] _addressBuffer = new int[8];
    private ArraySegment<int> _address = default;
    private WalkState _walkState = WalkState.None;

    private readonly RawWalkContext _rawWalkContext;

    public PauseStateProvidingWalkContext(RawWalkContext raw)
    {
        _rawWalkContext = raw;
    }

    public IBoundNode? BoundNode => _boundNode;
    public ReadOnlySpan<int> Address => _address;
    public WalkState WalkState => _walkState;

    public void OnWalked(IBoundNode boundNode, ReadOnlySpan<int> address)
    {
        _rawWalkContext.OnWalked(boundNode, address);
    }

    public WalkState GetRequiredState(IBoundNode boundNode, ReadOnlySpan<int> address)
    {
        _walkState = _rawWalkContext.GetRequiredState(boundNode, address);
        return _walkState;
    }

    public void OnPaused(IBoundNode boundNode, ReadOnlySpan<int> address)
    {
        _rawWalkContext.OnPaused(boundNode, address);

        _boundNode = boundNode;

        int addingLength = 0;
        while (_addressBuffer.Length + addingLength < address.Length)
        {
            addingLength += 8;
        }
        if (addingLength > 0)
        {
            _addressBuffer = new int[_addressBuffer.Length + addingLength];
        }
        address.CopyTo(_addressBuffer);
        _address = new ArraySegment<int>(_addressBuffer, 0, address.Length);
    }

    public void OnWalkEntering()
    {
        _rawWalkContext.OnWalkEntering();
        _walkState = WalkState.None;
    }
}