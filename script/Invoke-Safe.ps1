
[CmdletBinding()]
[OutputType([string])]
param (
    [Parameter(Mandatory, Position=0)]
    [scriptblock] $ScriptBlock
)

[string]$out = & $ScriptBlock 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error $out
}

return $out