﻿using cog1.DTO;
using cog1.Exceptions;

namespace cog1.Business
{
    /// <summary>
    /// Business to manage users and permissions
    /// </summary>
    public class UserBusiness : BusinessBase
    {

        public UserBusiness(Cog1Context context) : base(context)
        {

        }

        #region private


        #endregion

        #region CRUD

        public bool ValidateUserCredentials(string userName, string password, out UserDTO userData)
        {
            return Context.UserDao.ValidateUserCredentials(userName, password, out userData);
        }

        public bool TryGetUser(int userId, out UserDTO user)
        {
            user = Context.UserDao.GetUser(userId);
            return user != null;
        }

        private UserDTO GetUser(int userId)
        {
            var result = Context.UserDao.GetUser(userId);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.User.UNKNOWN_USER_ID);
            return result;
        }

        #endregion

    }
}