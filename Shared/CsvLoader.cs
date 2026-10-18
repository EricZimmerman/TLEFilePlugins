using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ITLEFileSpec;
using Serilog;
using Serilog.Events;

namespace TLEFilePlugins.Shared
{
    /// <summary>
    ///     Loads a CSV with a header row into a plugin's DataList (the code every CSV plugin repeated). Compiled into
    ///     each plugin assembly (a linked file), so Timeline Explorer needs nothing new
    /// </summary>
    internal static class CsvLoader
    {
        /// <summary>
        ///     A time from a converter: DateTime.Parse (the current culture), but DateTime.MinValue for an empty or unreadable
        ///     value (DateTime.Parse threw, and that failed the whole file)
        /// </summary>
        public static DateTime ParseTime(string text)
        {
            return DateTime.TryParse(text, out var d) ? d : DateTime.MinValue;
        }

        /// <summary>
        ///     Reads every record of <paramref name="filename" /> into <paramref name="dataList" />, numbering the lines from
        ///     1 and tagging those in <paramref name="taggedLines" />
        /// </summary>
        /// <remarks>
        ///     A plugin matches several header lines (each version of a tool's CSV), so the data class has the columns of
        ///     all of them: a column a file doesn't have keeps its default value (it was a HeaderValidationException).
        ///     Columns the class doesn't have are ignored. Time stamps are read as written (no time zone conversion)
        /// </remarks>
        /// <param name="fixup">Called for each record before it is added</param>
        public static void Load<T>(string filename, IBindingList dataList, IEnumerable<int> taggedLines,
            Action<ClassMap<T>> configureMap = null, Action<CsvConfiguration> configure = null, Action<T> fixup = null) where T : IFileSpecData
        {
            dataList.Clear();

            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(taggedLines ?? Array.Empty<int>());

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                //TLE matches header lines ignoring case, so the columns are too
                PrepareHeaderForMatch = args => args.Header.ToLowerInvariant()
            };
            configure?.Invoke(config);

            using var fileReader = File.OpenText(filename);
            using var csv = new CsvReader(fileReader, config);

            var map = csv.Context.AutoMap<T>();
            map.Map(t => t.Line).Ignore();
            map.Map(t => t.Tag).Ignore();
            configureMap?.Invoke(map);

            //an empty value in a number, date or flag column is that type's default (one empty value failed the whole
            //file: Amcache's DriverPackages Date, DeviceContainers State)
            foreach (var memberMap in map.MemberMaps)
            {
                var type = memberMap.Data.Type;
                if (type != null && type.IsValueType && Nullable.GetUnderlyingType(type) == null && !memberMap.Data.IsDefaultSet)
                {
                    memberMap.Data.Default = Activator.CreateInstance(type);
                    memberMap.Data.IsDefaultSet = true;
                }
            }

            csv.Context.RegisterClassMap(map);

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            var list = dataList as BindingList<T>;
            var raise = list?.RaiseListChangedEvents ?? false;
            if (list != null)
            {
                list.RaiseListChangedEvents = false;
            }

            try
            {
                var ln = 1;
                foreach (var record in csv.GetRecords<T>())
                {
                    //In TLE, there is an option to enable Debug messages which lets more context be seen when errors happen. The last known good line of data is then obvious in TLE messages
                    if (debug)
                    {
                        Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);
                    }

                    fixup?.Invoke(record);

                    record.Line = ln;
                    record.Tag = tagged.Contains(ln);
                    dataList.Add(record);

                    ln += 1;
                }
            }
            finally
            {
                if (list != null)
                {
                    list.RaiseListChangedEvents = raise;
                    if (raise)
                    {
                        list.ResetBindings();
                    }
                }
            }
        }
    }
}
