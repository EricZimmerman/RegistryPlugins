<#
.SYNOPSIS
    Copies the Release builds of every plugin to a folder, such as RECmd's or Registry Explorer's Plugins folder.

.DESCRIPTION
    Copies each plugin project's own RegistryPlugin.*.dll from bin\Release\netstandard2.0. The hosts already have
    RegistryPluginBase, Registry and their other dependencies, and plugins that need more (AppCompatCache) embed it.

.PARAMETER Out
    The folder to copy the plugins to. It is created when it doesn't exist.

.PARAMETER Build
    Build the solution in Release first.

.EXAMPLE
    .\CopyPlugins.ps1 -Out C:\Tools\RECmd\Plugins -Build
#>
param(
    [Parameter(Mandatory = $true)]
    [string] $Out,

    [switch] $Build
)

$ErrorActionPreference = 'Stop'

if ($Build) {
    dotnet build (Join-Path $PSScriptRoot 'RegistryPlugins.slnx') --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "The build failed" }
}

# one dll per plugin project: the one named after the project (its bin folder has RegistryPluginBase and others too)
$plugins = foreach ($project in Get-ChildItem -Path $PSScriptRoot -Filter '*.csproj' -Recurse -Depth 1) {
    $dll = Join-Path $project.DirectoryName "bin/Release/netstandard2.0/$($project.BaseName).dll"
    if ($project.BaseName -like 'RegistryPlugin.*' -and (Test-Path $dll)) { Get-Item $dll }
}

if (-not $plugins) {
    throw "No Release builds found. Build first (dotnet build -c Release), or use -Build"
}

New-Item -ItemType Directory -Path $Out -Force | Out-Null

foreach ($plugin in $plugins) {
    Copy-Item -Path $plugin.FullName -Destination $Out -Force
}

Write-Host "Copied $(@($plugins).Count) plugins to $((Resolve-Path $Out).Path)"
