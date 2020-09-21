#! /bin/pwsh

param(
    [string] $MsExtVer="3.1.5",
    [string] $EPPlusVer="5.3.2",
    [string] $rootDir = ".."
)

function Copy-MsExt([string] $dep, [string] $platform = "netcoreapp3.1") {
    cp ~/.nuget/packages/microsoft.extensions.$($dep)/$($MsExtVer)/lib/$($platform)/* $rootDir/Build/bin/ProteusWorkstation/Debug/netcoreapp3.1
}

Copy-Copy-MsExt "Primitives"
Copy-Copy-MsExt "Configuration.Abstractions"
Copy-Copy-MsExt "Configuration"
Copy-Copy-MsExt "Configuration.Json"
Copy-Copy-MsExt "Configuration.FileExtensions"
Copy-Copy-MsExt "FileSystemGlobbing" "netstandard2.0"
Copy-Copy-MsExt "FileProviders.Abstractions"
Copy-Copy-MsExt "FileProviders.Physical"

cp ~/.nuget/packages/epplus/$EPPlusVer/lib/netstandard2.1/* $rootDir/Build/bin/ProteusWorkstation/Debug/netcoreapp3.1