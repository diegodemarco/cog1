using cog1.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace cog1.Business
{
    /// <summary>
    /// Business to manage users and permissions
    /// </summary>
    public class SecurityBusiness : BusinessBase
    {
        public SecurityBusiness(Cog1Context context) : base(context)
        {

        }

        #region private

        private const long EXPIRATION_MS = 100 * 60 * 60;    // 1 hour

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private static Stopwatch swExpiration = Stopwatch.StartNew();
        private static Dictionary<Guid, TokenEntry> tokens = new();

        private class TokenEntry
        {
            public int userId;
            public long expiration;
        }

        private bool FindToken(Guid token, bool renew, out int userId)
        {
            semaphore.Wait();
            try
            {
                if (tokens.TryGetValue(token, out var entry) && entry.expiration > swExpiration.ElapsedMilliseconds)
                {
                    userId = entry.userId;
                    if (renew)
                        entry.expiration = swExpiration.ElapsedMilliseconds + EXPIRATION_MS;
                    return true;
                }
                userId = 0;
                return false;
            }
            finally
            {
                semaphore.Release();
            }
        }

        #endregion

        #region CRUD

        public Guid CreateAccessToken(int userId)
        {
            var token = Guid.NewGuid();
            semaphore.Wait();
            try
            {
                tokens[token] = new TokenEntry()
                {
                    userId = userId,
                    expiration = swExpiration.ElapsedMilliseconds + EXPIRATION_MS
                };
                return token;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public bool ValidateAccessToken(Guid token, out UserDTO user)
        {
            if (!FindToken(token, true, out var userId))
            {
                user = null;
                return false;
            }

            if (Context.UserBusiness.TryGetUser(userId, out user))
                return true;

            return false;
        }

        #endregion

    }
}
