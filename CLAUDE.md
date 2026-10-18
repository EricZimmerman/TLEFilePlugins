# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Plugins for Timeline Explorer (TLE, the `timelineexplorer` repository): each one turns a CSV (or text) format into typed rows. TLE loads every DLL in its `Plugins` folder and creates every `IFileSpec` type in them; it references `ITLEFileSpec` and `TLEFileTimelines` itself, and ships CsvHelper, Serilog and the other dependencies.

- `ITLEFileSpec` (`netstandard2.0`): `IFileSpec` (ExpectedHeaders, DataList, TaggedLines, InternalGuid, ProcessFile) and `IFileSpecData` (Line, Tag). Its interface is TLE's contract: don't change it.
- Plugin projects (`netstandard2.0`, one assembly each): `TLEFileEZTools` (Eric's tools), `TLEFileMisc` (third-party tools: Chainsaw, Hayabusa, BrowsingHistoryView, ...), `TLEFileKAPE`, `TLEFileMicrosoft`, `TLEFileTimelines`, `TLEFileTzWorks`, `TLEFileGenericCsv` (any other CSV/TSV; detects `,` tab `|` `;` from the header line, PR #18).
- **Matching**: TLE reads the file's first line and takes the first plugin whose `ExpectedHeaders` (a case-insensitive set of whole header lines) contains it; else GenericCsv. When a tool adds columns, **add its new header line to the same plugin and keep the old ones** (files from older tool versions), and add the new properties at the end of the data class (before Line/Tag) and to its `ToString` (TLE searches it). A new CSV gets a new plugin with a new `InternalGuid` (unique across all plugins; the tests check).
- **`Shared/CsvLoader.cs`** (linked into each plugin project, so nothing new ships): `CsvLoader.Load<T>(filename, DataList, TaggedLines, map, configure, fixup)` does what every plugin repeated, plus: a column the file doesn't have keeps its default (`HeaderValidated`/`MissingFieldFound` null: older header versions), columns match ignoring case (as TLE matches headers), an empty value in a non-nullable value column is its default (one empty value failed the whole file), tagged lines in a HashSet, no ListChanged event per row, `RawRecord` only built for debug messages. Times are read as written (`DateTimeStyles.None`; the old `AssumeUniversal & AdjustToUniversal` was 0 and registered for the record type, so it never applied). `CsvLoader.ParseTime` for converters (empty or bad text is MinValue). Plugins that parse by hand (TzWorks, Timelines, DensityScout, AnalyzeMft, Shimcache*, Sigcheck*, Autoruns, ConvertPSHistoryToCSV) don't use it.
- **Layouts** (`Layouts/Default/<namespace>.<class>.layout`, DevExpress grid layouts): TLE restores `Layouts\<name>.layout.local` (the user's) or `Layouts\Default\<name>.layout`; columns not in a layout are added at the end. Every EZ Tools plugin has one and every layout has all its plugin's columns (tests). The files came from Eric's TLE folder in 2026-10; `CopyPlugins.ps1 -Layouts` copies them.

## Commands

The SDK is pinned in `global.json` (.NET 10). Tests use NUnit 5 + FluentAssertions 7 (pinned to `[7.2.2,8.0.0)`, since 8.x requires a commercial license).

```bash
dotnet build TLEFilePlugins.slnx -c Release
dotnet test TLEFilePlugins.slnx -c Release          # CI (test.yml)
pwsh ./CopyPlugins.ps1 -Out <TLE>\Plugins -Layouts <TLE>\Layouts [-Build]
```

## Tests (`TLEFile.Test`, net10.0)

- `PluginTests`: unique InternalGuids; no header line claimed by two plugins; for every plugin (except the hand-parsed ones) and each of its header lines, a synthesized file (typed values by property name) loads 3 rows and tags; `TestFiles/CurrentHeaders.tsv` (the header lines of every CSV the 2026.10 tools write; header lines only, no data) all go to a plugin; layouts match plugins and have every column; `RealOutputs` (`[Explicit]`, `TLE_CSV_DIR`) loads real tool outputs.
- `LoaderTests`: older header versions, empty values, header case, times as written, GenericCsv delimiters.
- When a tool changes its CSV: run it, add its header line to `CurrentHeaders.tsv`, then to the plugin, then the layout.

## Versioning and release

No package or version: TLE ships the plugin DLLs (built here, copied to its `Plugins` folder) and the layouts.
