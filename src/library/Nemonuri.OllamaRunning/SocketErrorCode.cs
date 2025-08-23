using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Nemonuri.OllamaRunning;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct SocketErrorCode : IEquatable<SocketErrorCode>
{
    private readonly int _value;

    private SocketErrorCode(int value)
    {
        _value = value;
    }

    public static implicit operator SocketErrorCode(int value) => new(value);
    public static implicit operator int(SocketErrorCode source) => source._value;

    public bool Equals(SocketErrorCode other)
    {
        return _value == other._value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SocketErrorCode code && Equals(code);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
    
    public static bool operator ==(SocketErrorCode left, SocketErrorCode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SocketErrorCode left, SocketErrorCode right)
    {
        return !(left == right);
    }
}