using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Nemonuri.OllamaRunning;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Win32ErrorCode : IEquatable<Win32ErrorCode>
{
    private readonly int _value;

    private Win32ErrorCode(int value)
    {
        _value = value;
    }

    public static implicit operator Win32ErrorCode(int value) => new(value);
    public static implicit operator int(Win32ErrorCode source) => source._value;

    public bool Equals(Win32ErrorCode other)
    {
        return _value == other._value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Win32ErrorCode code && Equals(code);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
    
    public static bool operator ==(Win32ErrorCode left, Win32ErrorCode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Win32ErrorCode left, Win32ErrorCode right)
    {
        return !(left == right);
    }
}