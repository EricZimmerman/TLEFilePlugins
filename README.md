# TLEFilePlugins
Plugins for parsing CSV files in Timeline Explorer. This project allows for anyone to add more supported files (i,e. they get a Line #/tag column, layout support, searching, etc.)

I will maintain a reference to CSVHelper in TLE proper, so you do not need to worry about moving around that dll or any files it needs, just your plugins for file support.

Layouts, pinned columns, etc all rely on a call to **Plugin.GetType().FullName**, so name your classes according to the examples. The default layouts are in `Layouts\Default`, one per plugin, named `<namespace>.<class>.layout`.

## Writing a plugin

A plugin matches a file when the file's first line is one of its `ExpectedHeaders` (ignoring case). When a tool adds columns to its CSV, add the new header line to the same plugin (keep the old ones, so files from older versions still load) and the new columns at the end of the data class: a column a file doesn't have keeps its default value.

For CSVs with a header row, `ProcessFile` is one call:

    public void ProcessFile(string filename)
    {
        CsvLoader.Load<BootData>(filename, DataList, TaggedLines);
    }

with optional column maps (`map => map.Map(t => t.Timestomped).Name("SI<FN")`), CsvHelper settings (`configure: c => c.Delimiter = "\t"`) and a per-record fixup. `CsvLoader` (in `Shared`, compiled into each plugin assembly) numbers and tags the lines and reads the times as written.

For a complete example, see the Boot* classes at the link below.

https://github.com/EricZimmerman/TLEFilePlugins/blob/master/TLEFileEZTools/EZTools.cs#L12

For more advanced uses of CSVHelper, look at things like **AnalyzeMFT** or **Supertimeline** plugins.

## Building and installing

    dotnet build TLEFilePlugins.slnx -c Release
    dotnet test TLEFilePlugins.slnx -c Release
    .\CopyPlugins.ps1 -Out C:\Tools\TimelineExplorer\Plugins -Layouts C:\Tools\TimelineExplorer\Layouts

To make the layouts saved in Timeline Explorer (`Layouts\*.layout.local`) the defaults in `Layouts/Default` (only the ones that changed, and only for plugins in this solution; a saved filter, find text, sorting or grouping is reported, since it becomes the default too):

    .\CopyPlugins.ps1 -ImportLayouts C:\Tools\TimelineExplorer
