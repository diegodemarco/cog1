namespace cog1.Literals
{
    public class VariablesLiteralsContainer : BaseLiteralsContainer
    {
        public VariablesLiteralsContainer() : base() { }
        public VariablesLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Variable { get => new VariablesLiterals.Variable().ExtractLiteral(LocaleCode); set { } }
        public virtual string Variables { get => new VariablesLiterals.Variables().ExtractLiteral(LocaleCode); set { } }
        public virtual string NewVariable { get => new VariablesLiterals.NewVariable().ExtractLiteral(LocaleCode); set { } }
        public virtual string EditVariable { get => new VariablesLiterals.EditVariable().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteVariable { get => new VariablesLiterals.DeleteVariable().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteVariableConfirmation { get => new VariablesLiterals.DeleteVariableConfirmation().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableId { get => new VariablesLiterals.VariableId().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableType { get => new VariablesLiterals.VariableType().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableSource { get => new VariablesLiterals.VariableSource().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableAccessType { get => new VariablesLiterals.VariableAccessType().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableCode { get => new VariablesLiterals.VariableCode().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableUnits { get => new VariablesLiterals.VariableUnits().ExtractLiteral(LocaleCode); set { } }
        public virtual string PollInterval { get => new VariablesLiterals.Pollnterval().ExtractLiteral(LocaleCode); set { } }
        public virtual string Binary { get => new VariablesLiterals.Binary().ExtractLiteral(LocaleCode); set { } }
        public virtual string Integer { get => new VariablesLiterals.Integer().ExtractLiteral(LocaleCode); set { } }
        public virtual string FLoatingPoint { get => new VariablesLiterals.FloatingPoint().ExtractLiteral(LocaleCode); set { } }
        public virtual string Readonly { get => new VariablesLiterals.Readonly().ExtractLiteral(LocaleCode); set { } }
        public virtual string ReadWrite { get => new VariablesLiterals.ReadWrite().ExtractLiteral(LocaleCode); set { } }
        public virtual string ReadWriteAction { get => new VariablesLiterals.ReadWriteAction().ExtractLiteral(LocaleCode); set { } }
        public virtual string NoVariablesToDisplay { get => new VariablesLiterals.NoVariablesToDisplay().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableCreated { get => new VariablesLiterals.VariableCreated().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableUpdated { get => new VariablesLiterals.VariableUpdated().ExtractLiteral(LocaleCode); set { } }
        public virtual string VariableDeleted { get => new VariablesLiterals.VariableDeleted().ExtractLiteral(LocaleCode); set { } }
        public virtual string BuiltIn { get => new VariablesLiterals.BuiltIn().ExtractLiteral(LocaleCode); set { } }
        public virtual string Calculated { get => new VariablesLiterals.Calculated().ExtractLiteral(LocaleCode); set { } }
        public virtual string Modbus { get => new VariablesLiterals.Modbus().ExtractLiteral(LocaleCode); set { } }
        public virtual string External { get => new VariablesLiterals.External().ExtractLiteral(LocaleCode); set { } }
    }

}
