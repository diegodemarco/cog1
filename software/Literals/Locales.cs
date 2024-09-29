using System.Collections.Generic;

namespace cog1.Literals
{

    public static class Locales
    {
        public static readonly Locale English = new()
        {
            LocaleCode = "en",
            DefaultDescription = "English",
            BrowserLanguages = "en-us,en",
            IsDefault = true
        };

        public static readonly Locale Spanish = new()
        {
            LocaleCode = "es",
            DefaultDescription = "Español",
            BrowserLanguages = "es-ar,es-419,es-mx,es",
            IsDefault = false
        };

        public static readonly Locale Portuguese = new()
        {
            LocaleCode = "pt",
            DefaultDescription = "Português",
            BrowserLanguages = "pt-br,pt",
            IsDefault = false
        };

        public static readonly List<Locale> All = new()
        {
            English, Spanish, Portuguese
        };

    }

}
