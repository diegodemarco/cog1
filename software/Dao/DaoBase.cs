using cog1.Business;
using Microsoft.Extensions.Logging;

namespace cog1.Dao
{
    public class DaoBase
    {
        protected Cog1Context Context { get; }
        protected ILogger Logger { get; }

        public DaoBase(Cog1Context context, ILogger logger)
        {
            Context = context;
            Logger = logger;
        }

    }
}
