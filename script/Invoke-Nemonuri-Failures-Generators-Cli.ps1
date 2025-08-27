[CmdletBinding(PositionalBinding = $false)]
param (
    [switch]$Build,

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ToolArguments
)

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

[string]$projectName = "Nemonuri.Failures.Generators.Cli"

$workspaceRoot = Join-Path $PSScriptRoot ".." -Resolve
$projectFile = Join-Path $workspaceRoot "src" "tool" $projectName "$projectName.csproj" -Resolve

if ($Build) {
    & dotnet build $projectFile
    if ($LASTEXITCODE -ne 0) { exit 1 }
}

[string]$targetPath = & dotnet msbuild $projectFile -getProperty:TargetPath
if ($LASTEXITCODE -ne 0) { exit 1 }

& dotnet $targetPath @ToolArguments

