using System.Collections.Generic;
using System.ComponentModel;

namespace ITLEFileSpec
{
    /// <summary>
    ///     Note that it is also critical to override ToString and concatenate all fields in your class! This is used when
    ///     searching across each supported file type.
    ///     <remarks>
    ///         It is also useful to use DateTime vs DateTimeOffset so TLE filtering works better
    ///     </remarks>
    ///     <example>
    ///         See BootOut for fully commented example and others in CoreFileTypes for common patterns
    ///     </example>
    /// </summary>
    public interface IFileSpec
    {
        /// <summary>
        ///     Who wrote this amazing plugin (this would be you)
        /// </summary>
        string Author { get; }

        /// <summary>
        ///     A brief summary of what this file is, what generates it, version info, etc.
        /// </summary>
        string FileDescription { get; }


        /// <summary>
        ///     This should be the exact headers this type expects to process. These values are compared to what is in a file being
        ///     loaded.
        ///     <remarks>
        ///         Case insensitive
        ///     </remarks>
        /// </summary>
        HashSet<string> ExpectedHeaders { get; }

        /// <summary>
        ///     This should be a BindingList<YourIFileSpecDataClass> where records from the data source are loaded into
        /// </summary>
        IBindingList DataList { get; }

        /// <summary>
        ///     This will be populated with previously tagged lines (when session files are used). This allows for 're-tagging'
        ///     previously tagged rows.
        /// </summary>
        List<int> TaggedLines { get; }

        /// <summary>
        ///     When a matching ExpectedHeader is found, this will be called, passing the full path to the file.
        ///     <remarks>
        ///         This function should load the data in whatever way it deems necessary, adding each row to DataList
        ///     </remarks>
        /// </summary>
        /// <param name="filename"></param>
        void ProcessFile(string filename);
    }

    /// <summary>
    ///     Note that it is also critical to override ToString and concatenate all fields in your class! This is used when
    ///     searching across each supported file type.
    ///     <remarks>
    ///         It is also useful to use DateTime vs DateTimeOffset so TLE filtering works better
    ///     </remarks>
    ///     <example>
    ///         See BootOut for fully commented example and others in CoreFileTypes for common patterns
    ///     </example>
    /// </summary>
    public interface IFileSpecData
    {
        int Line { get; set; }
        bool Tag { get; set; }
    }
}