
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Diagnostics;

namespace Nemonuri.OllamaRunning;

public static partial class ProcessStartingTheory
{

    public static StartOrFailResult
    StartOrFail(this Process process)
    {
        Guard.IsNotNull(process);

        try
        {
            return StartOrFailResult.CreateAsValue(process.Start());
        }
        catch (ObjectDisposedException e)
        {
            return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.Disposed, e.Message);
        }
        catch (InvalidOperationException e)
        {
            var startInfo = process.StartInfo;
            if (startInfo.FileName.Length == 0)
            {
                return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.FileNameMissing, e.Message);
            }
            if (startInfo.StandardInputEncoding != null && !startInfo.RedirectStandardInput)
            {
                return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.StandardInputEncodingNotAllowed, e.Message);
            }
            if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
            {
                return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.StandardOutputEncodingNotAllowed, e.Message);
            }
            if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
            {
                return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.StandardErrorEncodingNotAllowed, e.Message);
            }
            if (!string.IsNullOrEmpty(startInfo.Arguments) && startInfo.ArgumentList.Count > 0)
            {
                return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.ArgumentAndArgumentListInitialized, e.Message);
            }
            throw;
        }
        catch (ArgumentNullException e)
        {
            return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.ArgumentListMayNotContainNull, e.Message);
        }
        catch (Win32Exception e)
        {
            return StartOrFailResult.CreateAsFileOpeningError(e.NativeErrorCode, e.Message);
        }
        catch (PlatformNotSupportedException e)
        {
            return StartOrFailResult.CreateAsFailure(StartOrFailResult.FailInfo.PlatformNotSupported, e.Message);
        }
    }
}

