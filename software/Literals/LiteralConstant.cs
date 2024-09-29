using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace cog1.Literals
{

    public abstract class LiteralConstant
    {
        public class EmptyLiteralConstant : LiteralConstant
        {
            public override string EN { get { return ""; } }
        }

        private static EmptyLiteralConstant _empty = new();

        public abstract string EN { get; }
        public virtual string ES { get { return ""; } }
        public virtual string PT { get { return ""; } }

        public static LiteralConstant Empty => _empty;

        public string ExtractLiteral(string localeCode) => ExtractLiteral(localeCode, this);
        public string ExtractLiteral(string localeCode, string defaultString) => ExtractLiteral(localeCode, this, true, defaultString);
        public string Format(string localeCode, object value1) => string.Format(ExtractLiteral(localeCode), value1);
        public string Format(string localeCode, object value1, object value2) => string.Format(ExtractLiteral(localeCode), value1, value2);
        public string Format(string localeCode, object value1, object value2, object value3) => string.Format(ExtractLiteral(localeCode), value1, value2, value3);

        //public List<LocalizedLiteral> ToLocalizedLiterals()
        //{
        //    var result = new List<LocalizedLiteral>();
        //    if (!string.IsNullOrEmpty(EN)) result.Add(new LocalizedLiteral() { LocaleCode = "en", Value = EN });
        //    if (!string.IsNullOrEmpty(ES)) result.Add(new LocalizedLiteral() { LocaleCode = "es", Value = ES });
        //    if (!string.IsNullOrEmpty(PT)) result.Add(new LocalizedLiteral() { LocaleCode = "pt", Value = PT });
        //    return result;
        //}

        public static string ExtractLiteral(string localeCode, LiteralConstant literals, bool tryOtherLanguages, string defaultString)
        {
            if (literals == null)
                return defaultString;

            // Requested language
            if (string.Equals(localeCode, Locales.English.LocaleCode, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(literals.EN))
                return literals.EN;
            if (string.Equals(localeCode, Locales.Spanish.LocaleCode, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(literals.ES))
                return literals.ES;
            if (string.Equals(localeCode, Locales.Portuguese.LocaleCode, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(literals.PT))
                return literals.PT;

            if (!tryOtherLanguages)
                return defaultString;

            // Other languages: try English first
            if (!string.IsNullOrEmpty(literals.EN))
                return literals.EN;

            // Other languages: try Spanish second
            if (!string.IsNullOrEmpty(literals.ES))
                return literals.ES;

            // Other languages: try Portuguese third
            if (!string.IsNullOrEmpty(literals.PT))
                return literals.PT;

            // No luck
            return defaultString;
        }

        public static string ExtractLiteral(string localeCode, LiteralConstant literals)
        {
            return ExtractLiteral(localeCode, literals, true, string.Empty);
        }

        public static string Serialize(Dictionary<string, string> literals)
        {
            return JsonConvert.SerializeObject(literals);
        }

        public static string Serialize(LiteralConstant literals)
        {
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(literals.EN))
                dict.Add(Locales.English.LocaleCode, literals.EN);
            if (!string.IsNullOrEmpty(literals.ES))
                dict.Add(Locales.Spanish.LocaleCode, literals.ES);
            if (!string.IsNullOrEmpty(literals.PT))
                dict.Add(Locales.Portuguese.LocaleCode, literals.PT);
            return JsonConvert.SerializeObject(literals);
        }

    }

}
