# TLEFilePlugins
Plugins for parsing CSV files in Timeline Explorer. This project allows for anyone to add more supported files (i,e. they get a Line #/tag column, layout support, searching, etc.)

For a complete example, see TLEFileMFTECmd, Boot* classes. For more advanced uses of CSVHelper, look at things like **AnalyzeMFT** or **Supertimeline** plugins.

I will maintain a reference to CSVHelper in TLE proper, so you do not need to worry about moving around that dll or any files it needs, just your plugins for file support.

Layouts, pinned columns, etc all rely on a call to **Plugin.GetType().FullName**, so name your classes according to the examples.
