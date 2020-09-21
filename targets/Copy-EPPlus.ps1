#! /bin/pwsh

param(
    [string] $MsExtVer="3.1.5",
    [string] $EPPlusVer="5.3.2",
    [string] $rootDir = ".."
)

function Copy-MsExt([string] $dep, [string] $platform = "netcoreapp3.1") {
    cp ~/.nuget/packages/microsoft.extensions.$($dep)/$($MsExtVer)/lib/$($platform)/* $rootDir/Build/bin/ProteusWorkstation/Debug/netcoreapp3.1
}

Copy-MsExt "Primitives"
Copy-MsExt "Configuration.Abstractions"
Copy-MsExt "Configuration"
Copy-MsExt "Configuration.Json"
Copy-MsExt "Configuration.FileExtensions"
Copy-MsExt "FileSystemGlobbing" "netstandard2.0"
Copy-MsExt "FileProviders.Abstractions"
Copy-MsExt "FileProviders.Physical"

cp ~/.nuget/packages/epplus/$EPPlusVer/lib/netstandard2.1/* $rootDir/Build/bin/ProteusWorkstation/Debug/netcoreapp3.1
