<#
.SYNOPSIS
    Copies the Release builds of the plugins, and optionally the layouts, to Timeline Explorer's folders.

.DESCRIPTION
    Copies each plugin project's own TLEFile*.dll from bin\Release\netstandard2.0 to -Out (Timeline Explorer's Plugins
    folder). Timeline Explorer already has ITLEFileSpec, CsvHelper, Serilog and the other dependencies. With -Layouts,
    also copies Layouts\Default\*.layout to <Layouts>\Default (Timeline Explorer's Layouts folder) and Layouts\!!README.txt
    to <Layouts>.

    With -ImportLayouts, does the opposite for the layouts: copies the layouts saved in Timeline Explorer
    (<namespace>.<class>.layout.local) to Layouts\Default as <namespace>.<class>.layout, so they become the defaults.
    Only the ones that differ from the default are copied, and only for a plugin in this solution. A sort by Line
    alone (the file's order) is removed. It reports a filter, find text, other sorting or grouping saved in a layout,
    since those become the default too.

.PARAMETER Out
    The folder to copy the plugins to. It is created when it doesn't exist.

.PARAMETER Layouts
    Timeline Explorer's Layouts folder; the default layouts go in its Default subfolder.

.PARAMETER Build
    Build the solution in Release first.

.PARAMETER ImportLayouts
    Timeline Explorer's folder, or its Layouts folder, to import the saved layouts from.

.EXAMPLE
    .\CopyPlugins.ps1 -Out C:\Tools\TimelineExplorer\Plugins -Layouts C:\Tools\TimelineExplorer\Layouts -Build

.EXAMPLE
    .\CopyPlugins.ps1 -ImportLayouts C:\Tools\TimelineExplorer
#>
param(
    [Parameter(Mandatory = $true, ParameterSetName = 'Copy')]
    [string] $Out,

    [Parameter(ParameterSetName = 'Copy')]
    [string] $Layouts,

    [Parameter(ParameterSetName = 'Copy')]
    [switch] $Build,

    [Parameter(Mandatory = $true, ParameterSetName = 'Import')]
    [string] $ImportLayouts
)

$ErrorActionPreference = 'Stop'

# what a saved layout would also make the default: a filter, find text, sorting or grouping
function Get-LayoutNotes([string] $path) {
    [xml]$layout = Get-Content -LiteralPath $path -Raw
    $root = $layout.DocumentElement
    $notes = @()
    $filter = $root.SelectSingleNode("property[@name='ActiveFilterString']")
    if ($filter -and $filter.InnerText) { $notes += "filter: $($filter.InnerText)" }
    $find = $root.SelectSingleNode("property[@name='FindFilterText']")
    if ($find -and $find.InnerText) { $notes += "find text: $($find.InnerText)" }
    $sort = $root.SelectSingleNode("property[@name='SortInfoState']")
    if ($sort) {
        $columns = @($sort.SelectNodes("property[starts-with(@name, 'Item')]/property[@name='Columns']") |
            ForEach-Object { $_.InnerText -replace '^col', '' })
        $groupCount = $sort.SelectSingleNode("property[@name='Content']/property[@name='GroupCount']")
        $groups = if ($groupCount) { [int]$groupCount.InnerText } else { 0 }
        if ($groups -gt 0) { $notes += "grouped by: $($columns[0..($groups - 1)] -join ', ')" }
        if ($columns.Count -gt $groups) { $notes += "sorted by: $($columns[$groups..($columns.Count - 1)] -join ', ')" }
    }
    $notes
}

# a sort by Line alone is the file's order, which every grid has unsorted; it only slows loading a large file
function Remove-LineSort([byte[]] $bytes) {
    $bom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
    $encoding = [Text.UTF8Encoding]::new($bom)
    $text = $encoding.GetString($bytes, $(if ($bom) { 3 } else { 0 }), $bytes.Length - $(if ($bom) { 3 } else { 0 }))
    $sort = '\r?\n[ \t]*<property name="SortInfoState" iskey="true" value="1">\s*' +
        '<property name="Item1" isnull="true" iskey="true">\s*<property name="Orders">\w+</property>\s*' +
        '<property name="Columns">colLine</property>\s*</property>\s*' +
        '<property name="Content" isnull="true" iskey="true">\s*<property name="GroupCount">0</property>\s*</property>\s*' +
        '</property>'
    $new = [regex]::Replace($text, $sort, '')
    if ($new -ceq $text) { return , $bytes }
    , [byte[]]($encoding.GetPreamble() + $encoding.GetBytes($new))
}

if ($PSCmdlet.ParameterSetName -eq 'Import') {
    $source = $ImportLayouts
    if (-not (Get-ChildItem -LiteralPath $source -Filter *.layout.local) -and (Test-Path (Join-Path $source 'Layouts'))) {
        $source = Join-Path $source 'Layouts'
    }
    $default = Join-Path $PSScriptRoot 'Layouts/Default'
    $saved = @(Get-ChildItem -LiteralPath $source -Filter *.layout.local)
    if (-not $saved) { throw "No saved layouts (*.layout.local) in $source" }

    $imported = 0
    $unchanged = 0
    foreach ($file in $saved) {
        $name = $file.BaseName # <namespace>.<class>.layout
        $target = Join-Path $default $name
        $exists = Test-Path -LiteralPath $target
        if (-not $exists) {
            # a plugin without a default layout yet: its class must be in its namespace's project
            $namespace, $class = ([IO.Path]::GetFileNameWithoutExtension($name)) -split '\.', 2
            $project = Join-Path $PSScriptRoot $namespace
            $found = (Test-Path $project) -and
                (Get-ChildItem $project -Filter *.cs -Recurse | Select-String -Pattern "class\s+$([regex]::Escape($class))\s*:\s*IFileSpec\b" -Quiet)
            if (-not $found) {
                Write-Warning "Skipped $($file.Name): no plugin $namespace.$class in this solution"
                continue
            }
        }

        [byte[]]$bytes = Remove-LineSort ([IO.File]::ReadAllBytes($file.FullName))
        if ($exists -and [Linq.Enumerable]::SequenceEqual($bytes, [IO.File]::ReadAllBytes($target))) {
            $unchanged++
            continue
        }

        [IO.File]::WriteAllBytes($target, $bytes)
        $imported++
        Write-Host "$(if ($exists) { 'Updated' } else { 'Added  ' }) $name"
        foreach ($note in Get-LayoutNotes $target) { Write-Host "          $note" -ForegroundColor Yellow }
    }
    Write-Host "Imported $imported layouts to $((Resolve-Path $default).Path) ($unchanged unchanged)"
    return
}

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
    # what the Default folder is, for users (next to it, as in the release)
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Layouts/!!README.txt') -Destination $Layouts -Force
}
