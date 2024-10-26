using cog1.DTO;
using cog1.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace cog1.Business
{
    /// <summary>
    /// Business to manage users and permissions
    /// </summary>
    public class UserBusiness : BusinessBase
    {

        public UserBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {

        }

        #region private


        #endregion

        #region CRUD

        public bool ValidateUserCredentials(string userName, string password, out UserDTO userData)
        {
            return Context.UserDao.ValidateUserCredentials(userName, password, out userData);
        }

        public List<UserDTO> EnumerateUsers()
        {
            return Context.UserDao.EnumerateUsers();
        }

        public bool TryGetUser(int userId, out UserDTO user)
        {
            user = Context.UserDao.GetUser(userId);
            return user != null;
        }

        public UserDTO GetUser(int userId)
        {
            var result = Context.UserDao.GetUser(userId);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Users.UNKNOWN_USER_ID);
            return result;
        }

        private void ValidateUser(UserDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.userName))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Username));
            if (user.localeCode == null)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Language));
            var loc = Context.MasterEntityBusiness.EnumerateLocales().FirstOrDefault(item => item.localeCode.Equals(user.localeCode.Trim(), System.StringComparison.OrdinalIgnoreCase));
            if (loc == null)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Language));
            user.localeCode = loc.localeCode;
            user.userName = user.userName.Trim();
        }

        public UserDTO CreateUser(UserWithPasswordDTO user)
        {
            // Validate
            if (user.user == null)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA("user"));
            if (string.IsNullOrWhiteSpace(user.password))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Password));
            ValidateUser(user.user);

            return GetUser(Context.UserDao.CreateUser(user));
        }

        public UserDTO EditUser(UserWithPasswordDTO user)
        {
            // Validate
            if (user.user == null)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA("user"));
            var currentUser = GetUser(user.user.userId);
            user.user.userName = currentUser.userName;       // Cannot change
            if (user.user.userId == 1)
            {
                user.user.isOperator = true;
                user.user.isAdmin = true;
            }
            ValidateUser(user.user);
            Context.UserDao.EditUser(user);
            return GetUser(user.user.userId);
        }

        public void DeleteUser(int userId)
        {
            // Validate
            GetUser(userId);
            if (userId < 1000)
                throw new ControllerException(Context.ErrorCodes.Users.CANNOT_DELETE_USER);
            Context.UserDao.DeleteUser(userId);
        }

        public void UpdateUserProfile(int userId, string localeCode)
        {
            if (string.IsNullOrWhiteSpace(localeCode))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Language));
            Context.UserDao.UpdateUserProfile(userId, localeCode);
        }

        #endregion

    }
}
