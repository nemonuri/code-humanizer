# How to run manual test

1. Set environment variable `MANUAL_TEST` to non-empty value

2. Run `dotnet run`, to use in-process console runner
   - do not use msbuild runner (`dotnet test`)
   - do not use vstest runner

## Example (Powershell)

```pwsh
$env:MANUAL_TEST = 1

dotnet run --project ./Nemonuri.OllamaRunning.ManualTests.csproj
```