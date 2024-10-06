using Microsoft.Extensions.Logging;

namespace cog1.Business
{
    public class BusinessBase
    {
        protected Cog1Context Context { get; }
        protected ILogger Logger { get; }

        public BusinessBase(Cog1Context serviceMethodContext, ILogger logger)
        {
            Context = serviceMethodContext;
            Logger = logger;
        }

        public virtual void DoHousekeeping()
        {

        }

        public virtual void DoStartupFixes()
        {

        }

    }
}