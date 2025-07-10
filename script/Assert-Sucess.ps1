
[CmdletBinding()]
param (
    [Parameter(Mandatory, Position=0)]
    [System.IO.FileInfo] $ProjectFile,
    [Parameter(Mandatory, Position=1)]
    [scriptblock] $Script,
    [Parameter(Mandatory, Position=2)]
    [string] $Result,
    [Parameter(Position=3)]
    [string] $Message = "",
    [Parameter()]
    [bool[]] $AddtionalConditions = @()
)

if ($LASTEXITCODE -eq 0) {
    return
}

if (-not ($AddtionalConditions -contains $false)) {
    return
}

if ($Message -ne "") {
    Write-Error $Message
}

Out-Host -InputObject @{
    ProjectFile = $ProjectFile
    Script = $script1
    Result = $result1
}
exit 1