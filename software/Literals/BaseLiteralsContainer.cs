#pragma warning disable 1591

namespace cog1.Literals
{
    public class BaseLiteralsContainer
    {
        private string _localeCode;
        protected string LocaleCode { get => _localeCode; }

        public BaseLiteralsContainer() { _localeCode = "en"; }
        public BaseLiteralsContainer(string localeCode) { _localeCode = localeCode; }

    }

}

#pragma warning restore 1591