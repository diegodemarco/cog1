#pragma warning disable 1591

namespace cog1.Literals
{
    public class LiteralsContainerDTO : BaseLiteralsContainer
    {
        private CommonLiteralsContainer _Common;
        //private GeneralParametersLiteralsContainer _GeneralParameters;

        public LiteralsContainerDTO() : base() { }
        public LiteralsContainerDTO(string localeCode) : base(localeCode) { }

        public CommonLiteralsContainer Common
        {
            get
            {
                if (_Common == null)
                    _Common = new CommonLiteralsContainer(LocaleCode);
                return _Common;
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

#pragma warning restore 1591