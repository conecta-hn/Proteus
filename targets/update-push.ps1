param(
    [System.String]$branch
)

function Get-ScriptDirectory
{
  $Invocation = (Get-Variable MyInvocation -Scope 1).Value
  Split-Path $Invocation.MyCommand.Path
}

$buildPath = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)\..\Build\bin\$project\Debug\netcoreapp3.1")
$targetPath = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)\..\Build\bin\$target\Debug\netcoreapp3.1")

if (-not [System.String]::IsNullOrWhiteSpace($project))
{
    Invoke-Command "git chechout"
}