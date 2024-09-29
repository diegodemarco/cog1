using cog1.Business;

namespace cog1.Dao
{
    public class DaoBase
    {
        protected Cog1Context Context { get; }

        public DaoBase(Cog1Context context)
        {
            Context = context;
        }

    }
}
