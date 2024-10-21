using cog1.DTO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace cog1.Business
{
    /// <summary>
    /// Business to manage users and permissions
    /// </summary>
    public class SecurityBusiness : BusinessBase
    {
        public SecurityBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {

        }

        #region private

        private object _lock = new object();
        private const long EXPIRATION_S = 24 * 60 * 60;     // 24 hours
        private static string accessTokensFileName = Path.Combine(Global.DataDirectory, "access_tokens.json");
        //private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private static Dictionary<Guid, AccessTokenEntry> accessTokens = null;


        private class AccessTokenEntry
        {
            public int userId;
            public DateTime expiration;
        }

        private static void LoadAccessTokensNoLock()
        {
            if (accessTokens == null)
            {
                if (File.Exists(accessTokensFileName))
                {
                    accessTokens = JsonConvert.DeserializeObject<Dictionary<Guid, AccessTokenEntry>>(File.ReadAllText(accessTokensFileName));
                }
                else
                {
                    accessTokens = new Dictionary<Guid, AccessTokenEntry>();
                }
            }
        }

        private static void StoreAccessTokensNoLock()
        {
            File.WriteAllText(accessTokensFileName, JsonConvert.SerializeObject(accessTokens));
        }

        private bool FindAccessToken(Guid token, bool renew, out int userId)
        {
            lock (_lock)
            {
                LoadAccessTokensNoLock();
                if (accessTokens.TryGetValue(token, out var entry) && entry.expiration > DateTime.UtcNow)
                {
                    userId = entry.userId;
                    if (renew)
                        entry.expiration = DateTime.UtcNow.AddSeconds(EXPIRATION_S);
                    return true;
                }
                userId = 0;
                return false;
            }
        }

        #endregion

        #region CRUD

        public Guid CreateAccessToken(int userId)
        {
            var token = Guid.NewGuid();
            lock (_lock)
            {
                LoadAccessTokensNoLock();
                accessTokens[token] = new AccessTokenEntry()
                {
                    userId = userId,
                    expiration = DateTime.UtcNow.AddSeconds(EXPIRATION_S)
                };
                StoreAccessTokensNoLock();
                return token;
            }
        }

        public bool ValidateAccessToken(Guid token, out UserDTO user)
        {
            if (!FindAccessToken(token, true, out var userId))
            {
                user = null;
                return false;
            }

            if (Context.UserBusiness.TryGetUser(userId, out user))
                return true;

            return false;
        }

        #endregion

        #region Startup fixes and housekeeping

        public override void DoHousekeeping()
        {
            lock (_lock) 
            {
                LoadAccessTokensNoLock();

                // Remove expired access tokens, and persist
                var expired = accessTokens.Where(item => item.Value.expiration < DateTime.UtcNow).ToList();
                if (expired.Count == 0)
                {
                    Logger.LogInformation($"ScurityBusiness: no expired tokens");
                }
                else
                {
                    foreach (var t in expired)
                        accessTokens.Remove(t.Key);
                    Logger.LogInformation($"Removed {expired.Count} expired token(s)");
                }

                // Persist any pending changes (expirations)
                StoreAccessTokensNoLock();
            }
        }

        #endregion

    }
}
