# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Plugins for Timeline Explorer (TLE, the `timelineexplorer` repository): each one turns a CSV (or text) format into typed rows. TLE loads every DLL in its `Plugins` folder and creates every `IFileSpec` type in them; it references `ITLEFileSpec` and `TLEFileTimelines` itself, and ships CsvHelper, Serilog and the other dependencies.

- `ITLEFileSpec` (`netstandard2.0`): `IFileSpec` (ExpectedHeaders, DataList, TaggedLines, InternalGuid, ProcessFile) and `IFileSpecData` (Line, Tag). Its interface is TLE's contract: don't change it.
- Plugin projects (`netstandard2.0`, one assembly each): `TLEFileEZTools` (Eric's tools), `TLEFileMisc` (third-party tools: Chainsaw, Hayabusa, BrowsingHistoryView, ...), `TLEFileKAPE`, `TLEFileMicrosoft`, `TLEFileTimelines`, `TLEFileTzWorks`, `TLEFileGenericCsv` (any other CSV/TSV; detects `,` tab `|` `;` from the header line, PR #18).
- **Matching**: TLE reads the file's first line and takes the first plugin whose `ExpectedHeaders` (a case-insensitive set of whole header lines) contains it; else GenericCsv. When a tool adds columns, **add its new header line to the same plugin and keep the old ones** (files from older tool versions), and add the new properties at the end of the data class (before Line/Tag) and to its `ToString` (TLE searches it). A new CSV gets a new plugin with a new `InternalGuid` (unique across all plugins; the tests check).
- **Variable headers**: iisGeolocate's CSVs are the IIS log's own `#Fields` plus `GeoCity,GeoCountry`, so `IisGeolocate` lists the IIS 6 and 7-10 defaults, every standard field, and variants seen in test logs (W3C names mapped to properties; `-` in a number field is null, `W3CNumberConverter`); another field list opens as generic, so add its header line. SQLECmd writes one CSV per map query (columns from the map's SQL, then SourceFile): only the ActivitiesCache map's three have plugins (`SqleActivitiesCache`, `SqleActivityPackageId`, `SqleActivityOperation`); the others open as generic.
- **`Shared/CsvLoader.cs`** (linked into each plugin project, so nothing new ships): `CsvLoader.Load<T>(filename, DataList, TaggedLines, map, configure, fixup)` does what every plugin repeated, plus: a column the file doesn't have keeps its default (`HeaderValidated`/`MissingFieldFound` null: older header versions), columns match ignoring case (as TLE matches headers), an empty value in a non-nullable value column is its default (one empty value failed the whole file), tagged lines in a HashSet, no ListChanged event per row, `RawRecord` only built for debug messages. Times are read as written (`DateTimeStyles.None`; the old `AssumeUniversal & AdjustToUniversal` was 0 and registered for the record type, so it never applied). `CsvLoader.ParseTime` for converters (empty or bad text is MinValue). Plugins that parse by hand (TzWorks, Timelines, DensityScout, AnalyzeMft, Shimcache*, Sigcheck*, Autoruns, ConvertPSHistoryToCSV) don't use it. They use `CsvLoader.QuietList<T>(DataList)` (no ListChanged event per row) and check `Log.IsEnabled(LogEventLevel.Debug)` once before logging each row's RawRecord. Every CsvReader has `CacheFields = true` (one string for a value repeated in many rows: about half the retained memory on large files, measured 2026-10-20).
- **Layouts** (`Layouts/Default/<namespace>.<class>.layout`, DevExpress grid layouts): TLE restores `Layouts\<name>.layout.local` (the user's) or `Layouts\Default\<name>.layout`; columns not in a layout are added at the end. Every EZ Tools plugin has one and every layout has all its plugin's columns (tests). The files came from Eric's TLE folder in 2026-10; `CopyPlugins.ps1 -Layouts` copies them.

## Commands

The SDK is pinned in `global.json` (.NET 10). Tests use NUnit 5 + FluentAssertions 7 (pinned to `[7.2.2,8.0.0)`, since 8.x requires a commercial license).

```bash
dotnet build TLEFilePlugins.slnx -c Release
dotnet test TLEFilePlugins.slnx -c Release          # CI (test.yml)
pwsh ./CopyPlugins.ps1 -Out <TLE>\Plugins -Layouts <TLE>\Layouts [-Build]
pwsh ./CopyPlugins.ps1 -ImportLayouts <TLE>     # TLE's saved layouts (*.layout.local) into Layouts/Default
```

## Tests (`TLEFile.Test`, net10.0)

- `PluginTests`: unique InternalGuids; no header line claimed by two plugins; for every plugin (except the hand-parsed ones) and each of its header lines, a synthesized file (typed values by property name) loads 3 rows and tags; `TestFiles/CurrentHeaders.tsv` (the header lines of every CSV the 2026.10 tools write; header lines only, no data) all go to a plugin; layouts match plugins and have every column; every layout starts with Line, then Tag (`LayoutsStartWithLineThenTag`); `RealOutputs` (`[Explicit]`, `TLE_CSV_DIR`) loads real tool outputs.
- `LoaderTests`: older header versions, empty values, header case, times as written, GenericCsv delimiters.
- When a tool changes its CSV: run it, add its header line to `CurrentHeaders.tsv`, then to the plugin, then the layout.

## Versioning and release

No package or version: TLE ships the plugin DLLs (built here, copied to its `Plugins` folder) and the layouts.
- **`Layouts/Default` is the source of truth for TLE's default layouts** (Eric, 2026-10). To make a layout saved in TLE the default, run `CopyPlugins.ps1 -ImportLayouts <TLE>` (copies each changed `Layouts\<namespace>.<class>.layout.local` here as `<namespace>.<class>.layout`, skips names with no plugin, reports a saved filter, find text, sorting or grouping), run the tests and commit. `PluginTests` fails for an EZ Tools plugin without a layout or a layout without a plugin.
- `Build-ZimmermanTools.ps1` (get-zimmermantools) runs `CopyPlugins.ps1 -Build` and stages the plugin DLLs in Timeline Explorer's `Plugins` and these layouts in its `Layouts\Default`: every plugin project in the solution ships.
