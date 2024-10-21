using cog1.Dao;
using cog1.DTO;
using cog1.Exceptions;
using cog1.Literals;
using Cog1.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace cog1.Business
{
    public class Cog1Context : IDisposable
    {
        private string localeCode = null;
        private Cog1DbContext databaseContext = null;
        private IHttpContextAccessor httpContextAccessor = null;
        private ILogger<Cog1Context> logger;

        public bool Committed { get; private set; } = false;

        public string LocaleCode => GetLocaleCode();

        private Lazy<HttpContext> httpContext;
        public HttpContext HttpContext => httpContext.Value;

        // Database
        public Cog1DbContext Db => GetDbContext();

        // Dao
        private Lazy<UserDao> userDao;
        public UserDao UserDao => userDao.Value;

        private Lazy<VariableDao> variableDao;
        public VariableDao VariableDao => variableDao.Value;

        // Businesses
        private Lazy<MasterEntityBusiness> masterEntityBusiness;
        public MasterEntityBusiness MasterEntityBusiness => masterEntityBusiness.Value;
        private Lazy<UserBusiness> userBusiness;
        public UserBusiness UserBusiness => userBusiness.Value;
        private Lazy<SecurityBusiness> securityBusiness;
        public SecurityBusiness SecurityBusiness => securityBusiness.Value;
        private Lazy<VariableBusiness> variableBusiness;
        public VariableBusiness VariableBusiness => variableBusiness.Value;

        // Security
        private UserDTO user = null;
        public UserDTO User => GetUser();

        // Literals
        private LiteralsContainerDTO literals = null;
        public LiteralsContainerDTO Literals => GetLiterals();

        // Error codes
        private Exceptions.ErrorCodes errorCodes = null;
        public Exceptions.ErrorCodes ErrorCodes => GetErrorCodes();

        public Cog1Context(IHttpContextAccessor httpContextAccessor, ILogger<Cog1Context> logger)
        {
            this.logger = logger;

            // Http context
            this.httpContextAccessor = httpContextAccessor;
            httpContext = new Lazy<HttpContext>(() => httpContextAccessor?.HttpContext);

            // Dao
            userDao = new Lazy<UserDao>(() => new UserDao(this, logger));
            variableDao = new Lazy<VariableDao>(() => new VariableDao(this, logger));

            // Business
            masterEntityBusiness = new Lazy<MasterEntityBusiness>(() => new MasterEntityBusiness(this, logger));
            userBusiness = new Lazy<UserBusiness>(() => new UserBusiness(this, logger));
            securityBusiness = new Lazy<SecurityBusiness>(() => new SecurityBusiness(this, logger));
            variableBusiness = new Lazy<VariableBusiness>(() => new VariableBusiness(this, logger));
        }

        protected virtual Exceptions.ErrorCodes GetErrorCodes()
        {
            if (errorCodes == null)
                errorCodes = new Exceptions.ErrorCodes(LocaleCode);
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
                if (!string.IsNullOrWhiteSpace(User?.localeCode))
                {
                    localeCode = User.localeCode;
                }
                else
                {
                    try
                    {
                        localeCode = MasterEntityBusiness.GetLocaleFromBrowser();
                        if (string.IsNullOrEmpty(localeCode))
                            localeCode = Locales.English.LocaleCode;
                    }
                    catch
                    {
                        localeCode = Locales.English.LocaleCode;
                    }
                }
            }
            return localeCode;
        }

        public void SetUser(UserDTO user)
        {
            this.user = user;

            // Update the locale (and the error codes if necessary) if the
            // new users's locale is different.
            if (localeCode != user.localeCode)
            {
                localeCode = user.localeCode;
                if (errorCodes != null && errorCodes.LocaleCode != localeCode)
                    errorCodes = null;
                if (literals != null && literals.LocaleCode != localeCode)
                    literals = null;
            }
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

        private LiteralsContainerDTO GetLiterals()
        {
            if (literals == null)
                literals = new LiteralsContainerDTO(LocaleCode);
            return literals;
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

        public List<BusinessBase> EnumerateBusinessObjects()
        {
            var result = new List<BusinessBase>();
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType.IsSubclassOf(typeof(BusinessBase)))
                    result.Add(prop.GetValue(this) as BusinessBase);
            }
            return result;
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