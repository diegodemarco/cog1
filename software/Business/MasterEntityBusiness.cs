using cog1.Literals;
using Microsoft.Extensions.Logging;

namespace cog1.Business
{
    /// <summary>
    /// Business to manage the Jobs raised by the Devices/Endpoints
    /// </summary>
    public class MasterEntityBusiness : BusinessBase
    {

        public MasterEntityBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {

        }

        #region Locales

        public string GetLocaleFromBrowser()
        {
            if (Context.HttpContext != null)
            {
                var request = Context.HttpContext.Request;
                if (request != null)
                {
                    // Search the supported browser languages against the system languages
                    var AcceptedLanguages = request.Headers["Accept-Language"];
                    if (!string.IsNullOrEmpty(AcceptedLanguages))
                    {
                        foreach (var lang in AcceptedLanguages.ToString().Split(','))
                        {
                            var code = lang.Split(';')[0].Trim().ToLower();
                            foreach (var l in Locales.All)
                            {
                                var c = ("," + l.BrowserLanguages + ",").Replace(" ", "").ToLower();
                                if (c.Contains("," + code + ","))
                                    return l.LocaleCode;
                            }
                        }
                    }
                }
            }

            // No match. Return default language.
            return GetDefaultLocale();
        }

        public string GetDefaultLocale()
        {
            var result = Locales.All.Find(item => item.IsDefault).LocaleCode;
            return string.IsNullOrWhiteSpace(result) ? Locales.English.LocaleCode : result;
        }

        #endregion

    }
}
