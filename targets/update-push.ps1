param(
    [System.String]$branch,
    [System.String]$publishPath = "D:\ProteusData"
)

function Get-ScriptDirectory
{
  $Invocation = (Get-Variable MyInvocation -Scope 1).Value
  Split-Path $Invocation.MyCommand.Path
}

$buildPath = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)\..\Build\bin\$project\Debug\netcoreapp3.1")
$targetPath = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)\..\Build\bin\$target\Debug\netcoreapp3.1")

$roth = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)/..").ToString()
Set-Location $roth

if (-not [System.String]::IsNullOrWhiteSpace($project))
{
    Invoke-Expression "git chechout $branch"
}

Invoke-Expression "git fetch"
Invoke-Expression "git pull"
Invoke-Expression "dotnet build $roth\src\Proteus.sln"
Copy-Item $([System.IO.path]::Combine($roth,"Build\bin\UpdatePusher\Debug\netcoreapp3.1\*")) $([System.IO.path]::Combine($roth,"Build\bin\ProteusWorkstation\Debug\netcoreapp3.1"))
Invoke-Expression "$([System.IO.path]::Combine($roth,"Build\bin\ProteusWorkstation\Debug\netcoreapp3.1\UpdatePusher.exe"))"
Copy-Item $([System.IO.path]::Combine($roth,"Build\bin\ProteusWorkstation\Debug\netcoreapp3.1\*")) $([System.IO.path]::Combine($publishPath,"release"))
Move-Item $([System.IO.path]::Combine($publishPath,"release\release.manifest")) $publishPath