namespace cog1.Business
{
    public class BusinessBase
    {
        protected Cog1Context Context { get; }

        public BusinessBase(Cog1Context serviceMethodContext)
        {
            Context = serviceMethodContext;
        }

    }
}