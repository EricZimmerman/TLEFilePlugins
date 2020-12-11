# TLEFilePlugins
Plugins for parsing CSV files in Timeline Explorer. This project allows for anyone to add more supported files (i,e. they get a Line #/tag column, layout support, searching, etc.)

I will maintain a reference to CSVHelper in TLE proper, so you do not need to worry about moving around that dll or any files it needs, just your plugins for file support.

Layouts, pinned columns, etc all rely on a call to **Plugin.GetType().FullName**, so name your classes according to the examples.

For a complete example, see TLEFileMFTECmd, Boot* classes at the link below. 

https://github.com/EricZimmerman/TLEFilePlugins/blob/master/TLEFileEZTools/EZTools.cs#L12

For more advanced uses of CSVHelper, look at things like **AnalyzeMFT** or **Supertimeline** plugins.
