namespace Nemonuri.OllamaRunning;

public static class OllamaRunningConstants
{
    public const string DefaultLocalOllamaHostCommand = "ollama";

    public static readonly Uri DefaultOllamaServerUri = new UriBuilder() { Port = 11434 }.Uri;
}