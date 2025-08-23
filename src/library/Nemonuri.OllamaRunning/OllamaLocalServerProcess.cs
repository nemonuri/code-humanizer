using System.Diagnostics;

namespace Nemonuri.OllamaRunning;

public class OllamaLocalServerProcess : IDisposable
{
    private readonly Process _process;
    private readonly string _listenerAddress;
    private readonly string _version;

    internal OllamaLocalServerProcess(Process process, string listenerAddress, string version)
    {
        _process = process;
        _listenerAddress = listenerAddress;
        _version = version;
    }

    public void Dispose()
    {
        _process.Dispose();
        GC.SuppressFinalize(this);
    }

    public override string ToString() =>
$"OllamaLocalServerProcess {{ Process = {_process}, ListenerAddress = {_listenerAddress}, Version = {_version} }}";
}

internal record OllamaLocalServerStartInfo(string ListenerAddress, string Version);