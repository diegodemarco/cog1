#pragma warning disable 1591

namespace cog1.Literals
{
    public class BaseLiteralsContainer
    {
        private string localeCode;
        public string LocaleCode => localeCode;

        public BaseLiteralsContainer() { localeCode = "en"; }
        public BaseLiteralsContainer(string localeCode) { this.localeCode = localeCode; }

    }

}

#pragma warning restore 1591