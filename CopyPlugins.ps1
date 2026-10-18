<#
.SYNOPSIS
    Copies the Release builds of the plugins, and optionally the layouts, to Timeline Explorer's folders.

.DESCRIPTION
    Copies each plugin project's own TLEFile*.dll from bin\Release\netstandard2.0 to -Out (Timeline Explorer's Plugins
    folder). Timeline Explorer already has ITLEFileSpec, CsvHelper, Serilog and the other dependencies. With -Layouts,
    also copies Layouts\Default\*.layout to <Layouts>\Default (Timeline Explorer's Layouts folder).

.PARAMETER Out
    The folder to copy the plugins to. It is created when it doesn't exist.

.PARAMETER Layouts
    Timeline Explorer's Layouts folder; the default layouts go in its Default subfolder.

.PARAMETER Build
    Build the solution in Release first.

.EXAMPLE
    .\CopyPlugins.ps1 -Out C:\Tools\TimelineExplorer\Plugins -Layouts C:\Tools\TimelineExplorer\Layouts -Build
#>
param(
    [Parameter(Mandatory = $true)]
    [string] $Out,

    [string] $Layouts,

    [switch] $Build
)

$ErrorActionPreference = 'Stop'

if ($Build) {
    dotnet build (Join-Path $PSScriptRoot 'TLEFilePlugins.slnx') --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "The build failed" }
}

# the plugin projects in the solution, and for each its own dll from its Release build
[xml]$solution = Get-Content (Join-Path $PSScriptRoot 'TLEFilePlugins.slnx') -Raw
$projects = $solution.SelectNodes('//Project') | ForEach-Object { Join-Path $PSScriptRoot $_.Path } |
    Where-Object { [IO.Path]::GetFileNameWithoutExtension($_) -like 'TLEFile*' -and [IO.Path]::GetFileNameWithoutExtension($_) -notlike '*.Test' }

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

if ($Layouts) {
    $default = Join-Path $Layouts 'Default'
    New-Item -ItemType Directory -Path $default -Force | Out-Null
    $files = Get-ChildItem (Join-Path $PSScriptRoot 'Layouts/Default') -Filter *.layout
    $files | Copy-Item -Destination $default -Force
    Write-Host "Copied $(@($files).Count) layouts to $((Resolve-Path $default).Path)"
}
