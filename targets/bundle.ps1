param(
    [System.String]$project,
    [System.String]$target = "ProteusWorkStation"
)

function Get-ScriptDirectory
{
  $Invocation = (Get-Variable MyInvocation -Scope 1).Value
  Split-Path $Invocation.MyCommand.Path
}

$buildPath = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)\..\Build\bin\$project\Debug\netcoreapp3.1")
$targetPath = [System.IO.path]::GetFullPath("$(Get-ScriptDirectory)\..\Build\bin\$target\Debug\netcoreapp3.1")

if ([System.String]::IsNullOrWhiteSpace($project))
{
    Write-Error "Se debe especificar un proyecto a incluir en el Bundle." -Category InvalidArgument
    return
}

foreach ($j in Get-ChildItem $buildPath)
{
    $file =[System.IO.Path]::Combine($targetPath, $j.Name)

    if (![System.IO.File]::Exists($file))
    {
        Write-Output "Copiando $($j.Name) a $target..."
        Copy-Item -Path $j.FullName -Destination $targetPath -Force -ErrorAction SilentlyContinue
    }
    else
    {
        if ($j.LastWriteTimeUtc -gt [System.IO.FileInfo]::new($file).LastWriteTimeUtc)
        {
            Write-Output "Actualizando $($j.Name) en $target..."
            Copy-Item -Path $j.FullName -Destination $targetPath -Force -ErrorAction SilentlyContinue
        }
    }
}