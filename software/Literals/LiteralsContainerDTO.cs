namespace cog1.Literals
{
    public class LiteralsContainerDTO : BaseLiteralsContainer
    {
        private CommonLiteralsContainer common;
        private DashboardLiteralsContainer dashboard;
        //private GeneralParametersLiteralsContainer _GeneralParameters;

        public LiteralsContainerDTO() : base() { }
        public LiteralsContainerDTO(string localeCode) : base(localeCode) { }

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

        //public GeneralParametersLiteralsContainer GeneralParameters
        //{
        //    get
        //    {
        //        if (_GeneralParameters == null)
        //            _GeneralParameters = new GeneralParametersLiteralsContainer(LocaleCode);
        //        return _GeneralParameters;
        //    }
        //    set { }
        //}

    }

}