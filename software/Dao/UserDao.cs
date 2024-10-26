using System.Collections.Generic;
using System.Data;
using cog1.Business;
using cog1.DTO;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using cog1.Exceptions;
using static cog1.Literals.CommonLiterals;

namespace cog1.Dao
{
    /// <summary>
    /// Dao class for handling the "User" entity
    /// </summary>
    public class UserDao : DaoBase
    {
        private object _lock = new object();
        private Dictionary<int, UserDTO> users = null;
        //private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public UserDao(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        #region Private methods

        private UserDTO MakeUser(DataRow r)
        {
            var result = new UserDTO
            {
                userId = (int)r.Field<long>("user_id"),
                userName = r.Field<string>("user_name"),
                isAdmin = r.Field<long>("is_admin") > 0,
                isOperator = r.Field<long>("is_operator") > 0,
                localeCode = r.Field<string>("locale_code"),
            };

            // Fixes
            if (result.isAdmin)
                result.isOperator = true;

            return result;
        }

        private void LoadUsers(bool reload = false)
        {
            lock (_lock)
            {
                if (reload && users != null)
                {
                    users.Clear();
                    users = null;
                }
                if (users == null)
                {
                    users = Context.Db.GetDataTable("select * from users")
                        .AsEnumerable()
                        .Select(row => MakeUser(row))
                        .ToDictionary(item => item.userId);
                }
            }
        }

        private List<UserDTO> _GetUsers()
        {
            LoadUsers();
            lock (_lock)
            {
                return users.Select(item => item.Value).ToList();      // Clone
            }
        }

        #endregion

        public List<UserDTO> EnumerateUsers()
        {
            return _GetUsers();
        }

        public UserDTO GetUser(int userId)
        {
            LoadUsers();
            lock (_lock)
            {
                if (users.TryGetValue(userId, out UserDTO user))
                    return user;
                return null;
            }
        }

        public UserDTO GetUser(string userName)
        {
            LoadUsers();
            lock (_lock)
            {
                return users.Values.FirstOrDefault(item => string.Equals(item.userName, userName, System.StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool ValidateUserCredentials(string userName, string password, out UserDTO userData)
        {
            userData = null;
            var user = GetUser(userName);
            if (user != null)
            {
                var dbHash = Context.Db.GetString("select password from users where user_id = @user_id",
                    new() { { "@user_id", user.userId } });
                if (dbHash == Utils.HashPassword(user.userId, password))
                    userData = user;
            }
            return userData != null;
        }

        public int CreateUser(UserWithPasswordDTO user)
        {
            int result;

            lock (_lock)
            {
                // Check duplicate user name
                if (GetUser(user.user.userName) != null)
                    throw new ControllerException(Context.ErrorCodes.Users.DUPLICATE_USERNAME);

                // Store
                result = users.Values.Select(item => item.userId).Max() + 1;
                if (result < 1001)
                    result = 1001;
                Context.Db.Execute(
                    "insert into users (user_id, user_name, password, is_admin, is_operator, locale_code) " +
                    "values (@user_id, @user_name, @password, @is_admin, @is_operator, @locale_code)",
                    new()
                    {
                        { "@user_id", result },
                        { "@user_name", user.user.userName },
                        { "@password", Utils.HashPassword(result, user.password) },
                        { "@is_admin", user.user.isAdmin },
                        { "@is_operator", user.user.isOperator || user.user.isAdmin },
                        { "@locale_code", user.user.localeCode }
                    });

                // Reload users
                LoadUsers(true);
            }
            return result;
        }

        public void EditUser(UserWithPasswordDTO user)
        {
            lock (_lock)
            {
                // Update user data
                Context.Db.Execute(
                    "update users set is_admin = @is_admin, is_operator = @is_operator, locale_code = @locale_code where user_id = @user_id",
                    new()
                    {
                        { "@is_admin", user.user.isAdmin },
                        { "@is_operator", user.user.isOperator || user.user.isAdmin },
                        { "@locale_code", user.user.localeCode },
                        { "@user_id", user.user.userId },
                    });

                // Change password if provided
                if (!string.IsNullOrWhiteSpace(user.password))
                {
                    Context.Db.Execute(
                        "update users set password = @password where user_id = @user_id",
                        new()
                        {
                            { "@user_id", user.user.userId },
                            { "@password", Utils.HashPassword(user.user.userId, user.password) }
                        });
                }

                // Reload users
                LoadUsers(true);
            }
        }

        public void DeleteUser(int userId)
        {
            Context.Db.Execute(
                "delete from users where user_id = @user_id",
                new()
                {
                    { "@user_id", userId },
                });
            LoadUsers(true);
        }

        public void UpdateUserProfile(int userId, string localeCode)
        {
            var user = GetUser(userId);
            if (!string.Equals(user.localeCode, localeCode, StringComparison.OrdinalIgnoreCase))
            {
                Context.Db.Execute("update users set locale_code = @locale_code where user_id = @user_id",
                    new()
                    {
                    { "@user_id", userId },
                    { "@locale_code", localeCode }
                    });
                LoadUsers(true);
            }
        }

        /*
        public int CreateAccount(Account account)
        {
            return DBProxy.CreateAccount(Context.Transaction, account.AccountId, account.Firstname, account.Lastname,
                account.EMail, account.Password, account.ActivationKey,
                account.EmailIsVerified, account.LocaleCode, account.IsSystemUser, account.PhoneNumber, account.TimeZoneCode);
        }

        public void StoreAccountProfileImage(Account account)
        {
            // null means "no changes"
            if (account.ProfilePictureBase64 == null)
                return;

            // We need to remove the existing profile image first
            var currentFile = Context.AccountDao.GetAccountProfilePicture(account.AccountId);
            if (!string.IsNullOrEmpty(currentFile))
                Context.BlobManager.DeletePublicObject(PROFILE_PICTURES_PATH + currentFile);

            // Empty string means "delete current profile image" (return to default image)
            if (account.ProfilePictureBase64 == "")
            {
                Context.AccountDao.SetAccountProfilePicture(account.AccountId, null);
                return;
            }

            // Anything else means "update image"
            var newFile = $"{account.AccountId}.{Guid.NewGuid().ToString("N")}.{account.ProfilePictureType.Replace(".", "")}";
            var imageBytes = Convert.FromBase64String(account.ProfilePictureBase64);
            Context.AccountDao.SetAccountProfilePicture(account.AccountId, newFile);
            Context.BlobManager.SetPublicObjectBytes(PROFILE_PICTURES_PATH + newFile, imageBytes);
        }

        public int EditAccount(Account account, Boolean inactiveAccount)
        {
            return DBProxy.EditAccount(Context.Transaction, account.AccountId, account.Firstname, account.Lastname,
                account.Password, account.LocaleCode, account.ActivationKey, account.Suspended, account.PhoneNumber,
                account.TimeZoneCode, inactiveAccount);
        }

        public void SetAccountApplicationData(int accountID, string applicationData)
        {
            if (Context.ApplicationID == 0)
                throw new ServiceException(Context.ErrorCodes.General.CUSTOM("Cannot store application data: not authenticated"));

            var Data = DBProxy.GetAccountApplicationData(Context.Transaction, accountID);
            var Entries = new List<AccountApplicationDataEntry>();
            if (Data.Trim() != "")
                Entries = JsonConvert.DeserializeObject<List<AccountApplicationDataEntry>>(Data);
            var Entry = Entries.Find(item => item.ApplicationID == Context.ApplicationID);
            if (Entry == null)
            {
                Entries.Add(new AccountApplicationDataEntry() { ApplicationID = Context.ApplicationID, ApplicationData = applicationData });
            }
            else
            {
                Entry.ApplicationData = applicationData;
            }
            DBProxy.SetAccountApplicationData(Context.Transaction, accountID, JsonConvert.SerializeObject(Entries));
        }

        public string GetAccountProfilePicture(int accountID)
        {
            var T = DBProxy.GetAccountProfilePicture(Context.Transaction, accountID);
            if (T.Rows.Count != 1)
                throw new ServiceException(Context.ErrorCodes.Account.UNKNOWN_ACCOUNT_ID);
            return T.Rows[0].Field<string>("ProfilePicture");
        }

        public void SetAccountProfilePicture(int accountID, string profilePicture)
        {
            if (DBProxy.SetAccountProfilePicture(Context.Transaction, accountID, profilePicture) != accountID)
                throw new ServiceException(Context.ErrorCodes.Account.UNKNOWN_ACCOUNT_ID);
        }

        public int EditBasicAccountData(Account account)
        {
            return DBProxy.EditBasicAccountData(Context.Transaction, account.AccountId, account.Firstname, account.Lastname,
                account.LocaleCode, account.EMail, account.PhoneNumber, account.TimeZoneCode);
        }

        public void DeleteAccount(int accountID)
        {
            if (DBProxy.DeleteAccount(Context.Transaction, accountID) <= 0)
                throw new ServiceException(Context.ErrorCodes.Account.UNKNOWN_ACCOUNT_ID);
        }

        public void ChangePassword(int accountID, string newPassword, bool invalidateTokens)
        {
            DBProxy.ChangeAccountPassword(Context.Transaction, accountID, newPassword, invalidateTokens);
        }

        public void InitiatePasswordReset(int accountID, string activationKey)
        {
            DBProxy.InitiateAccountPasswordReset(Context.Transaction, accountID, activationKey);
        }

        public int ResetPassword(string activationKey, string password)
        {
            return DBProxy.ResetAccountPassword(Context.Transaction, activationKey, password);
        }

        public int ValidateLogin(string eMail, string password)
        {
            var res = DBProxy.ValidateLogin(Context.Transaction, eMail, password);
            if (res > 0)
                return res;
            if (res == -1)
            {
                // The account exists, but the password is invalid. Maybe there's a migration process available?
                if (PivotSetup.Options.PasswordMigrationDelegate != null)
                {
                    var account = GetAccountFromEmail(eMail);
                    if (PivotSetup.Options.PasswordMigrationDelegate(Context, account, password))
                    {
                        // Migrate this password
                        ChangePassword(account.AccountId, password, false);
                        return account.AccountId;
                    }
                }
            }
            throw new ServiceException(Context.ErrorCodes.Account.INVALID_LOGIN_DETAILS);
        }

        public Account GetAccount(int accountID)
        {
            var T = DBProxy.GetAccount(Context.Transaction, accountID);
            if (T == null || T.Rows.Count != 1)
                throw new ServiceException(Context.ErrorCodes.Account.UNKNOWN_ACCOUNT_ID);
            return MakeAccount(T.Rows[0]);
        }

        public int GetAccountIDFromEmail(string eMail)
        {
            return DBProxy.GetAccountIDFromEMail(Context.Transaction, eMail);
        }

        public Account GetAccountFromEmail(string eMail)
        {
            var AccountID = DBProxy.GetAccountIDFromEMail(Context.Transaction, eMail);
            if (AccountID <= 0)
                throw new ServiceException(Context.ErrorCodes.Account.UNKNOWN_EMAIL_ADDRESS);
            return GetAccount(AccountID);
        }

        public int ActivateAccount(string activationKey)
        {
            return DBProxy.ActivateAccount(Context.Transaction, activationKey);
        }

        public int GetAccountIDforInactiveAccountFromEmail(string eMail)
        {
            return DBProxy.GetAccountIDforInactiveAccountFromEmail(Context.Transaction, eMail);
        }

        public LoginDetails GetLoginDetails(int accountID)
        {
            var T = DBProxy.GetLoginDetails(Context.Transaction, accountID);
            if (T == null || T.Rows.Count != 1)
                throw new ServiceException(Context.ErrorCodes.Account.UNKNOWN_ACCOUNT_ID);
            DataRow r = T.Rows[0];
            return new LoginDetails()
            {
                AccountId = accountID,
                Email = r.Field<string>("Email"),
                Firstname = r.Field<string>("Firstname"),
                Lastname = r.Field<string>("Lastname")
            };
        }

        public void PurgeExpiredAccessTokens()
        {
            DBProxy.PurgeExpiredAccessTokens(Context.Transaction);
        }
        */

    }

}
