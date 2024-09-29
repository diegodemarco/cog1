namespace cog1.Literals
{

    /// <summary>
    /// Represents an entry in the list of internal locales
    /// </summary>
    public class Locale
    {
        /// <summary>
        /// Unique locale code.
        /// </summary>
        public string LocaleCode { get; set; }

        /// <summary>
        /// Default description for this locale.
        /// </summary>
        public string DefaultDescription { get; set; }

        /// <summary>
        /// Comma-separated list of related browser languages.
        /// </summary>
        public string BrowserLanguages { get; set; }

        /// <summary>
        /// Indicates if this locale is to be used as default.
        /// </summary>
        public bool IsDefault { get; set; }
    }

}
