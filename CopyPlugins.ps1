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

# the plugin projects in the solution (a project left out of it, or an old build of one, is not copied), and for each
# its own dll from its Release build (the bin folder has RegistryPluginBase and others too)
[xml]$solution = Get-Content (Join-Path $PSScriptRoot 'RegistryPlugins.slnx') -Raw
$projects = $solution.SelectNodes('//Project') | ForEach-Object { Join-Path $PSScriptRoot $_.Path } |
    Where-Object { [IO.Path]::GetFileNameWithoutExtension($_) -like 'RegistryPlugin.*' }

$missing = @()
$plugins = foreach ($project in $projects) {
    $name = [IO.Path]::GetFileNameWithoutExtension($project)
    $dll = Join-Path (Split-Path $project) "bin/Release/netstandard2.0/$name.dll"
    if (Test-Path $dll) { Get-Item $dll } else { $missing += $name }
}

if ($missing) {
    throw "No Release build of $($missing -join ', '). Build first (dotnet build -c Release), or use -Build"
}
New-Item -ItemType Directory -Path $Out -Force | Out-Null

foreach ($plugin in $plugins) {
    Copy-Item -Path $plugin.FullName -Destination $Out -Force
}

Write-Host "Copied $(@($plugins).Count) plugins to $((Resolve-Path $Out).Path)"
