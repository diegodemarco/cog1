using cog1.Dao;
using cog1.DTO;
using cog1.Exceptions;
using Cog1.DB;
using Microsoft.AspNetCore.Http;
using System;

namespace cog1.Business
{
    public class Cog1Context : IDisposable
    {
        private string localeCode = null;
        private ErrorCodes errorCodes = null;
        private Cog1DbContext databaseContext = null;
        private IHttpContextAccessor httpContextAccessor = null;

        public bool Committed { get; private set; } = false;

        public ErrorCodes ErrorCodes => GetErrorCodes();

        public string LocaleCode => GetLocaleCode();

        private Lazy<HttpContext> httpContext;
        public HttpContext HttpContext => httpContext.Value;

        // Database
        public Cog1DbContext Db => GetDbContext();

        // Dao
        private Lazy<UserDao> userDao;
        public UserDao UserDao => userDao.Value;

        // Businesses
        private Lazy<MasterEntityBusiness> masterEntityBusiness;
        public MasterEntityBusiness MasterEntityBusiness => masterEntityBusiness.Value;
        private Lazy<UserBusiness> userBusiness;
        public UserBusiness UserBusiness => userBusiness.Value;
        private Lazy<SecurityBusiness> securityBusiness;
        public SecurityBusiness SecurityBusiness => securityBusiness.Value;

        // Security
        private UserDTO user = null;
        public UserDTO User => GetUser();


        public Cog1Context(IHttpContextAccessor httpContextAccessor)
        {
            // Http context
            this.httpContextAccessor = httpContextAccessor;
            httpContext = new Lazy<HttpContext>(() => httpContextAccessor?.HttpContext);

            // Dao
            userDao = new Lazy<UserDao>(() => new UserDao(this));

            // Business
            masterEntityBusiness = new Lazy<MasterEntityBusiness>(() => new MasterEntityBusiness(this));
            userBusiness = new Lazy<UserBusiness>(() => new UserBusiness(this));
            securityBusiness = new Lazy<SecurityBusiness>(() => new SecurityBusiness(this));
        }

        protected virtual ErrorCodes GetErrorCodes()
        {
            if (errorCodes == null)
                errorCodes = new ErrorCodes(LocaleCode);
            return errorCodes;
        }

        private Cog1DbContext GetDbContext()
        {
            if (databaseContext == null)
                databaseContext = Cog1DbContext.CreateInstance();
            return databaseContext;
        }


        private string GetLocaleCode()
        {
            if (localeCode == null)
            {
                if (!string.IsNullOrWhiteSpace(User.localeCode))
                {
                    localeCode = User.localeCode;
                }
                else
                {
                    try
                    {
                        localeCode = MasterEntityBusiness.GetLocaleFromBrowser();
                        if (string.IsNullOrEmpty(localeCode))
                            localeCode = Literals.Locales.English.LocaleCode;
                    }
                    catch
                    {
                        localeCode = Literals.Locales.English.LocaleCode;
                    }
                }
            }
            return localeCode;
        }

        public void SetUser(UserDTO user)
        {
            this.user = user;
        }

        private UserDTO GetUser()
        {
            if (user == null)
            {
                user = new UserDTO()
                {
                    userId = 0,
                    userName = string.Empty,
                    isAdmin = false,
                };
            }
            return user;
        }

        public void CheckIsAdmin()
        {
            if (!User.isAdmin)
                throw new ControllerException(ErrorCodes.Security.MUST_BE_ADMIN);
        }

        public void Commit()
        {
            // Use own database context
            if (databaseContext != null)
            {
                databaseContext.Commit();
                Committed = true;
            }
        }

        void IDisposable.Dispose()
        {
            if (databaseContext != null)
            {
                (databaseContext as IDisposable).Dispose();
                databaseContext = null;
            }
        }

    }
}