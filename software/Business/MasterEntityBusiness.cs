﻿using cog1.DTO;
using cog1.Literals;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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

        public List<LocaleDTO> EnumerateLocales()
        {
            return new List<LocaleDTO>
            {
                new ()
                {
                    localeCode = Locales.English.LocaleCode,
                    description = "English",
                },
                new ()
                {
                    localeCode = Locales.Spanish.LocaleCode,
                    description = "Español",
                },
                new ()
                {
                    localeCode = Locales.Portuguese.LocaleCode,
                    description = "Português",
                }
            };
        }

        public string GetLocaleFromBrowser()
        {
            if (Context.HttpContext != null)
            {
                // Logger.LogInformation("HttpContext is not null");
                var request = Context.HttpContext.Request;
                if (request != null)
                {
                    // Search the supported browser languages against the system languages
                    var acceptedLanguages = request.Headers["Accept-Language"];
                    if (!string.IsNullOrEmpty(acceptedLanguages))
                    {
                        foreach (var lang in acceptedLanguages.ToString().Split(','))
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
                else
                {
                    // Logger.LogInformation("Context.HttpContext.Request is null");
                }
            }
            else
            {
                // Logger.LogInformation("HttpContext is null");
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
