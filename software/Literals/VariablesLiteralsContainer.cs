namespace cog1.Literals
{
    public class VariablesLiteralsContainer : BaseLiteralsContainer
    {
        public VariablesLiteralsContainer() : base() { }
        public VariablesLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Variable { get => new VariablesLiterals.Variable().ExtractLiteral(LocaleCode); set { } }

        public virtual string Variables { get => new VariablesLiterals.Variables().ExtractLiteral(LocaleCode); set { } }

        public virtual string Binary { get => new VariablesLiterals.Binary().ExtractLiteral(LocaleCode); set { } }

        public virtual string Integer { get => new VariablesLiterals.Integer().ExtractLiteral(LocaleCode); set { } }

        public virtual string FLoatingPoint { get => new VariablesLiterals.FloatingPoint().ExtractLiteral(LocaleCode); set { } }

    }

}
