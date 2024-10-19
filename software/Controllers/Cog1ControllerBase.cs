using cog1.Business;
using cog1.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace cog1.Controllers
{
    public class Cog1ControllerBase
    {
        protected readonly Cog1Context Context;

        public Cog1ControllerBase(Cog1Context context)
        {
            Context = context;
        }

        #region Protected Methods

        protected void MethodPattern(Action action)
        {
            try
            {
                action();
            }
            catch (ControllerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ControllerException(e, Context.LocaleCode);
            }
        }

        protected T MethodPattern<T>(Func<T> action)
        {
            try
            {
                var result = action();
                return result;
            }
            catch (ControllerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ControllerException(e, Context.LocaleCode);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "pong";
        }

        #endregion Public Methods

        #region Async

        protected T MethodPatternAsync<T>(Func<Task<T>> action)
        {
            try
            {
                var result = action().Result;
                return result;
            }
            catch (ControllerException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                throw new ControllerException(e.InnerExceptions.First(), Context.LocaleCode);
            }
            catch (Exception e)
            {
                throw new ControllerException(e, Context.LocaleCode);
            }
        }

        #endregion Async
    }

}