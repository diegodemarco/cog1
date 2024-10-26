namespace cog1.Literals
{
    public class LiteralsContainerDTO : BaseLiteralsContainer
    {
        private CommonLiteralsContainer common;
        private DashboardLiteralsContainer dashboard;
        private SecurityLiteralsContainer security;
        private VariablesLiteralsContainer variables;

        public LiteralsContainerDTO() : base() 
        {
            Common = null;
        }

        public LiteralsContainerDTO(string localeCode) : base(localeCode) 
        { 
            
        }

        public CommonLiteralsContainer Common
        {
            get
            {
                if (common == null)
                    common = new CommonLiteralsContainer(LocaleCode);
                return common;
            }
            set { }
        }

        public DashboardLiteralsContainer Dashboard
        {
            get
            {
                if (dashboard == null)
                    dashboard = new DashboardLiteralsContainer(LocaleCode);
                return dashboard;
            }
            set { }
        }

        public SecurityLiteralsContainer Security
        {
            get
            {
                if (security == null)
                    security = new SecurityLiteralsContainer(LocaleCode);
                return security;
            }
            set { }
        }

        public VariablesLiteralsContainer Variables
        {
            get
            {
                if (variables == null)
                    variables = new VariablesLiteralsContainer(LocaleCode);
                return variables;
            }
            set { }
        }

    }

}