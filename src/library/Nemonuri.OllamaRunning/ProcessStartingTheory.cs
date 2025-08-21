
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using Nemonuri.Failures;

namespace Nemonuri.OllamaRunning;

public static class ProcessStartingTheory
{
    public static ValueOrFailure<bool, ProcessStartFailInfo>
    StartOrFail(this Process process)
    {
        Guard.IsNotNull(process);

        try
        {
            return process.Start();
        }
        catch (ObjectDisposedException e)
        {
            return Create(ProcessStartFailCode.Disposed, e.Message);
        }
        catch (InvalidOperationException e)
        {
            var startInfo = process.StartInfo;
            if (startInfo.FileName.Length == 0)
            {
                return Create(ProcessStartFailCode.FileNameMissing, e.Message);
            }
            if (startInfo.StandardInputEncoding != null && !startInfo.RedirectStandardInput)
            {
                return Create(ProcessStartFailCode.StandardInputEncodingNotAllowed, e.Message);
            }
            if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
            {
                return Create(ProcessStartFailCode.StandardOutputEncodingNotAllowed, e.Message);
            }
            if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
            {
                return Create(ProcessStartFailCode.StandardErrorEncodingNotAllowed, e.Message);
            }
            if (!string.IsNullOrEmpty(startInfo.Arguments) && startInfo.ArgumentList.Count > 0)
            {
                return Create(ProcessStartFailCode.ArgumentAndArgumentListInitialized, e.Message);
            }
            throw;
        }
        catch (ArgumentNullException e)
        {
            return Create(ProcessStartFailCode.ArgumentListMayNotContainNull, e.Message);
        }
        catch (Win32Exception e)
        {
            return Create(ProcessStartFailCode.FileOpeningError, e.NativeErrorCode, e.Message);
        }
        catch (PlatformNotSupportedException e)
        {
            return Create(ProcessStartFailCode.PlatformNotSupported, e.Message);
        }
    }

    private static Failure<ProcessStartFailInfo> Create
    (
        ProcessStartFailCode failCode,
        int win32ErrorCode,
        string message
    ) =>
    new Failure<ProcessStartFailInfo>(new ProcessStartFailInfo(failCode, win32ErrorCode), message);

    private static Failure<ProcessStartFailInfo> Create
    (
        ProcessStartFailCode failCode,
        string message
    ) =>
    Create(failCode, default, message);
}

public enum ProcessStartFailCode
{
    Unknown = 0,
    FileNameMissing = 1,
    StandardInputEncodingNotAllowed = 2,
    StandardOutputEncodingNotAllowed = 3,
    StandardErrorEncodingNotAllowed = 4,
    ArgumentAndArgumentListInitialized = 5,
    ArgumentListMayNotContainNull = 6,
    Disposed = 7,
    FileOpeningError = 8,
    PlatformNotSupported = 9
}

public readonly record struct ProcessStartFailInfo
{
    public ProcessStartFailInfo(ProcessStartFailCode failCode, int win32ErrorCode)
    {
        FailCode = failCode;
        Win32ErrorCode = win32ErrorCode;
    }

    public ProcessStartFailInfo(ProcessStartFailCode failCode) : this(failCode, default)
    { }

    public ProcessStartFailCode FailCode { get; }
    public int Win32ErrorCode { get; }
}