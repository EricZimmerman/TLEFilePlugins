using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using CsvHelper;
using ITLEFileSpec;
using Serilog;
using Serilog.Events;
using TLEFilePlugins.Shared;

//an empty value in a number, date or flag column is that type's default (one empty value failed the whole file:
//Amcache's DriverPackages Date, DeviceContainers State): a converter for each value type, for every [CsvRecord] type of
//the plugin assembly (a nullable member is null for an empty value before its converter is asked)
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Boolean))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Byte))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Int16))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.UInt16))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Int32))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.UInt32))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Int64))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.UInt64))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Single))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Double))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Decimal))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.DateTime))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.DateTimeOffset))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.Guid))]
[assembly: CsvConverter(typeof(EmptyIsDefaultConverters.TimeSpan))]
[assembly: CsvConverter(typeof(PooledStringConverter))]

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    /// <summary>CsvHelper 34's generated code registers its mappers with a module initializer, which netstandard2.0 lacks</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class ModuleInitializerAttribute : Attribute
    {
    }
}
#endif

namespace TLEFilePlugins.Shared
{
    /// <summary>
    ///     A value type's default for an empty field (CsvHelper reads it as a FormatException); anything else as CsvHelper
    ///     reads it (invariant culture). Each converter calls TryParse/Parse with its own type: CsvHelper's .NET build
    ///     constrains their type parameter (ISpanParsable), which a type parameter of a converter here doesn't satisfy
    /// </summary>
    internal static class EmptyIsDefaultConverters
    {
        internal sealed class Boolean : ICsvConverter<bool>
        {
            public bool Read(CsvReader reader, int index) =>
                reader.TryParse<bool>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<bool>(index);

            public void Write(CsvWriter writer, bool value) => writer.WriteField(value ? "True" : "False");
        }

        internal sealed class Byte : ICsvConverter<byte>
        {
            public byte Read(CsvReader reader, int index) =>
                reader.TryParse<byte>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<byte>(index);

            public void Write(CsvWriter writer, byte value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class Int16 : ICsvConverter<short>
        {
            public short Read(CsvReader reader, int index) =>
                reader.TryParse<short>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<short>(index);

            public void Write(CsvWriter writer, short value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class UInt16 : ICsvConverter<ushort>
        {
            public ushort Read(CsvReader reader, int index) =>
                reader.TryParse<ushort>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<ushort>(index);

            public void Write(CsvWriter writer, ushort value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class Int32 : ICsvConverter<int>
        {
            public int Read(CsvReader reader, int index) =>
                reader.TryParse<int>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<int>(index);

            public void Write(CsvWriter writer, int value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class UInt32 : ICsvConverter<uint>
        {
            public uint Read(CsvReader reader, int index) =>
                reader.TryParse<uint>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<uint>(index);

            public void Write(CsvWriter writer, uint value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class Int64 : ICsvConverter<long>
        {
            public long Read(CsvReader reader, int index) =>
                reader.TryParse<long>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<long>(index);

            public void Write(CsvWriter writer, long value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class UInt64 : ICsvConverter<ulong>
        {
            public ulong Read(CsvReader reader, int index) =>
                reader.TryParse<ulong>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<ulong>(index);

            public void Write(CsvWriter writer, ulong value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class Single : ICsvConverter<float>
        {
            public float Read(CsvReader reader, int index) =>
                reader.TryParse<float>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<float>(index);

            public void Write(CsvWriter writer, float value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class Double : ICsvConverter<double>
        {
            public double Read(CsvReader reader, int index) =>
                reader.TryParse<double>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<double>(index);

            public void Write(CsvWriter writer, double value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class Decimal : ICsvConverter<decimal>
        {
            public decimal Read(CsvReader reader, int index) =>
                reader.TryParse<decimal>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<decimal>(index);

            public void Write(CsvWriter writer, decimal value) => writer.WriteField(value.ToString(CultureInfo.InvariantCulture));
        }

        internal sealed class DateTime : ICsvConverter<System.DateTime>
        {
            public System.DateTime Read(CsvReader reader, int index) =>
                reader.TryParse<System.DateTime>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<System.DateTime>(index);

            public void Write(CsvWriter writer, System.DateTime value) => writer.WriteField(value.ToString("O", CultureInfo.InvariantCulture));
        }

        internal sealed class DateTimeOffset : ICsvConverter<System.DateTimeOffset>
        {
            public System.DateTimeOffset Read(CsvReader reader, int index) =>
                reader.TryParse<System.DateTimeOffset>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<System.DateTimeOffset>(index);

            public void Write(CsvWriter writer, System.DateTimeOffset value) => writer.WriteField(value.ToString("O", CultureInfo.InvariantCulture));
        }

        internal sealed class Guid : ICsvConverter<System.Guid>
        {
            public System.Guid Read(CsvReader reader, int index) =>
                reader.TryParse<System.Guid>(index, out var value) || reader.GetSpan(index).IsEmpty ? value : reader.Parse<System.Guid>(index);

            public void Write(CsvWriter writer, System.Guid value) => writer.WriteField(value.ToString());
        }

        /// <summary>TimeSpan.Parse with the invariant culture (as CsvHelper 33 read it)</summary>
        internal sealed class TimeSpan : ICsvConverter<System.TimeSpan>
        {
            public System.TimeSpan Read(CsvReader reader, int index)
            {
                var text = reader.GetString(index);
                return text.Length == 0 ? default : System.TimeSpan.Parse(text, CultureInfo.InvariantCulture);
            }

            public void Write(CsvWriter writer, System.TimeSpan value)
            {
                writer.WriteField(value.ToString("c", CultureInfo.InvariantCulture));
            }
        }
    }

    /// <summary>
    ///     One string for a value repeated in many rows, not one for each, whatever the column (TLE keeps every row: about
    ///     half the retained memory of a large file). CsvHelper 34's PoolStrings stops pooling a column where most values
    ///     are new, such as MFTECmd's FileName and ParentPath or EvtxECmd's Payload columns, which kept a third more memory
    ///     than CsvHelper 33's CacheFields; the values of up to 1024 characters are pooled here too. One pool for one file
    ///     (the plugins don't read records in parallel)
    /// </summary>
    internal sealed class StringPool
    {
        //longer values rarely repeat and cost more to hash
        private const int MaxPooledLength = 1024;

        private readonly Dictionary<string, string> _pool = new(StringComparer.Ordinal);

        public string Get(string value)
        {
            if (value == null || value.Length == 0 || value.Length > MaxPooledLength)
            {
                return value;
            }

            if (_pool.TryGetValue(value, out var pooled))
            {
                return pooled;
            }

            _pool.Add(value, value);
            return value;
        }
    }

    /// <summary>
    ///     A string field through a <see cref="StringPool" />: the generated mapper of one file's records creates its own
    ///     converters, so the pool is dropped after the load
    /// </summary>
    internal sealed class PooledStringConverter : ICsvConverter<string>
    {
        private readonly StringPool _pool = new();

        public string Read(CsvReader reader, int index) => _pool.Get(reader.GetString(index));

        public void Write(CsvWriter writer, string value) => writer.WriteField(value ?? string.Empty);
    }

    /// <summary>
    ///     A time written with ToString() (RECmd's plugin CSVs: the culture of the machine that ran it): the invariant
    ///     culture, then the current one; with an offset, the time as written. Empty or unreadable is null (one bad value
    ///     failed the file)
    /// </summary>
    internal sealed class LenientDateTimeConverter : ICsvConverter<DateTime?>
    {
        public DateTime? Read(CsvReader reader, int index)
        {
            var text = reader.GetString(index);
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            foreach (var culture in new[] {CultureInfo.InvariantCulture, CultureInfo.CurrentCulture})
            {
                if (DateTimeOffset.TryParse(text, culture, DateTimeStyles.AllowWhiteSpaces, out var dto))
                {
                    return dto.DateTime;
                }
            }

            return null;
        }

        public void Write(CsvWriter writer, DateTime? value)
        {
            writer.WriteField(value?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty);
        }
    }

    /// <summary>A time read with <see cref="CsvLoader.ParseTime" /> (the current culture; empty or unreadable is MinValue)</summary>
    internal sealed class ParseTimeConverter : ICsvConverter<DateTime>
    {
        public DateTime Read(CsvReader reader, int index) => CsvLoader.ParseTime(reader.GetString(index));

        public void Write(CsvWriter writer, DateTime value) => writer.WriteField(value.ToString("O", CultureInfo.InvariantCulture));
    }

    /// <inheritdoc cref="ParseTimeConverter" />
    internal sealed class NullableParseTimeConverter : ICsvConverter<DateTime?>
    {
        public DateTime? Read(CsvReader reader, int index) => CsvLoader.ParseTime(reader.GetString(index));

        public void Write(CsvWriter writer, DateTime? value) => writer.WriteField(value?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty);
    }

    /// <summary><see cref="CsvLoader.ParseTime" />, then ToUniversalTime (a time without a zone is taken as local)</summary>
    internal sealed class UniversalParseTimeConverter : ICsvConverter<DateTime>
    {
        public DateTime Read(CsvReader reader, int index) => CsvLoader.ParseTime(reader.GetString(index)).ToUniversalTime();

        public void Write(CsvWriter writer, DateTime value) => writer.WriteField(value.ToString("O", CultureInfo.InvariantCulture));
    }

    /// <summary><see cref="CsvLoader.ParseTime" /> of the value without its Zs (the time as written, not converted to local)</summary>
    internal sealed class NoZParseTimeConverter : ICsvConverter<DateTime>
    {
        public DateTime Read(CsvReader reader, int index) => CsvLoader.ParseTime(reader.GetString(index).Replace("Z", ""));

        public void Write(CsvWriter writer, DateTime value) => writer.WriteField(value.ToString("O", CultureInfo.InvariantCulture));
    }

    /// <inheritdoc cref="NoZParseTimeConverter" />
    internal sealed class NullableNoZParseTimeConverter : ICsvConverter<DateTime?>
    {
        public DateTime? Read(CsvReader reader, int index) => CsvLoader.ParseTime(reader.GetString(index).Replace("Z", ""));

        public void Write(CsvWriter writer, DateTime? value) => writer.WriteField(value?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty);
    }

    /// <summary>
    ///     Loads a CSV with a header row into a plugin's DataList (the code every CSV plugin repeated). Compiled into
    ///     each plugin assembly (a linked file), so Timeline Explorer needs nothing new
    /// </summary>
    internal static class CsvLoader
    {
        /// <summary>
        ///     The options every CSV plugin reads with: comma separated (no detection), malformed quoting recovered, a field
        ///     past the end of a record empty, and one string for a value repeated in many rows, not one for each (about
        ///     half the retained memory on large files)
        /// </summary>
        public static readonly CsvReaderOptions Options = new CsvReaderOptions().With(
            (nameof(CsvReaderOptions.Separator), ','),
            (nameof(CsvReaderOptions.PoolStrings), true),
            (nameof(CsvReaderOptions.MalformedData), CsvMalformedDataHandling.Recover),
            (nameof(CsvReaderOptions.MissingField), CsvMissingFieldHandling.ReturnEmpty));

        /// <summary>
        ///     A copy of <paramref name="options" /> with the given properties set (what <c>options with { ... }</c> does).
        ///     CsvReaderOptions' properties are init-only, and a netstandard2.0 plugin can't call their setters on the
        ///     .NET 10 CsvHelper that TLE ships: in CsvHelper's netstandard2.0 build the setters' signatures name its own
        ///     IsExternalInit, in the .NET one the runtime's (MissingMethodException). Reflection calls the setter TLE has
        /// </summary>
        public static CsvReaderOptions With(this CsvReaderOptions options, params (string Name, object Value)[] values)
        {
            var type = typeof(CsvReaderOptions);
            //the record's copy method, which `with` calls
            var copy = (CsvReaderOptions) type.GetMethod("<Clone>$")!.Invoke(options, null);
            foreach (var (name, value) in values)
            {
                (type.GetProperty(name) ?? throw new ArgumentException($"CsvReaderOptions has no {name}", nameof(values))).SetValue(copy, value);
            }

            return copy;
        }

        /// <summary>
        ///     For a plugin that adds its rows itself: no ListChanged event for each row while loading; when disposed, the
        ///     events are back as they were, with one reset if they were on (what <see cref="Load{T}" /> does)
        /// </summary>
        public static IDisposable QuietList<T>(IBindingList dataList)
        {
            return new ListChangedSuspension<T>(dataList as BindingList<T>);
        }

        private sealed class ListChangedSuspension<T> : IDisposable
        {
            private readonly BindingList<T> _list;
            private readonly bool _raise;

            public ListChangedSuspension(BindingList<T> list)
            {
                _list = list;
                if (list != null)
                {
                    _raise = list.RaiseListChangedEvents;
                    list.RaiseListChangedEvents = false;
                }
            }

            public void Dispose()
            {
                if (_list == null)
                {
                    return;
                }

                _list.RaiseListChangedEvents = _raise;
                if (_raise)
                {
                    _list.ResetBindings();
                }
            }
        }

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
        ///     <typeparamref name="T" /> is a [CsvRecord] type. A plugin matches several header lines (each version of a
        ///     tool's CSV), so the data class has the columns of all of them: its members are
        ///     [CsvColumn(Optional = true)], and a column a file doesn't have keeps its default value. Columns the class
        ///     doesn't have are ignored, header names match ignoring case (as TLE matches header lines), Line and Tag are
        ///     [CsvIgnore]. Time stamps are read as written (no time zone conversion)
        /// </remarks>
        /// <param name="options">The reader options; <see cref="Options" /> when null</param>
        /// <param name="fixup">Called for each record before it is added</param>
        public static void Load<T>(string filename, IBindingList dataList, IEnumerable<int> taggedLines,
            CsvReaderOptions options = null, Action<T> fixup = null) where T : IFileSpecData
        {
            dataList.Clear();

            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(taggedLines ?? Array.Empty<int>());

            using var csv = CsvReader.FromFile(filename, options ?? Options);

            //(the raw record is only built for debug messages)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = QuietList<T>(dataList);

            var ln = 1;
            foreach (var record in csv.GetRecords<T>())
            {
                //In TLE, there is an option to enable Debug messages which lets more context be seen when errors happen. The last known good line of data is then obvious in TLE messages
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                fixup?.Invoke(record);

                record.Line = ln;
                record.Tag = tagged.Contains(ln);
                dataList.Add(record);

                ln += 1;
            }
        }
    }
}
