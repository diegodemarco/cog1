namespace cog1.Literals
{
    public class SecurityLiteralsContainer : BaseLiteralsContainer
    {
        public SecurityLiteralsContainer() : base() { }
        public SecurityLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Security { get => new SecurityLiterals.Security().ExtractLiteral(LocaleCode); set { } }
        public virtual string Users { get => new SecurityLiterals.Users().ExtractLiteral(LocaleCode); set { } }

    }

}
