
[CmdletBinding()]
param (
    [Parameter(Mandatory, Position=0)]
    [System.IO.FileInfo] $ProjectFile
)

function Invoke-Safe([scriptblock] $ScriptBlock) {
    & "$PSScriptRoot/Invoke-Safe.ps1" $ScriptBlock
}

function Assert-Success(
    [System.IO.FileInfo] $ProjectFile,
    [scriptblock] $Script,
    [string] $Result,
    [string] $Message = "",
    [bool[]] $AddtionalConditions = @()
) {
    & "$PSScriptRoot/Assert-Sucess.ps1" $ProjectFile $Script $Result $Message -AddtionalConditions $AddtionalConditions
}

if (-not $ProjectFile.Exists) {
    Write-Error "$ProjectFile not exists."
    exit 1
}

#--- script1 ---
$script1 = { dotnet msbuild $ProjectFile -getProperty:UsingFStarLangSdk }
$result1 = Invoke-Safe $script1

Assert-Success $ProjectFile $script1 $result1 `
    "UsingFStarLangSdk property is not true." @($result1 -ieq "true")
#---|

#--- script2 ---
$script1 = { dotnet restore $ProjectFile }
$result1 = Invoke-Safe $script1

Assert-Success $ProjectFile $script1 $result1
#---|

#--- script3 ---
$propertyName = $IsWindows ? "PkgFStarLang_Compiler_runtime_win-x64" : "PkgFStarLang_Compiler_runtime_linux-x64"
$script1 = { dotnet msbuild $ProjectFile -getProperty:$propertyName }
$result1 = Invoke-Safe $script1

Assert-Success $ProjectFile $script1 $result1 `
    "$propertyName property is empty." @($result1 -ne "")
#---|

$pkgFStarLangCompilerruntime = $result1

try {
    $fstarExePath = Join-Path $pkgFStarLangCompilerruntime tools bin ("fstar" + ($IsWindows ? ".exe" : "")) -Resolve
    $includeDirs = @(
        Join-Path $pkgFStarLangCompilerruntime tools lib fstar -Resolve
    )

    $resultConfig = @{
        fstar_exe = $fstarExePath
        options = @("--cache_dir", ".cache.boot", "--no_location_info", "--warn_error", "-271-272-241-319-274")
        include_dirs = $includeDirs
    }

    ConvertTo-Json $resultConfig | Out-File "$PSScriptRoot/../.fst.config.json"
} catch {
    Write-Error $_
    exit 1
}



