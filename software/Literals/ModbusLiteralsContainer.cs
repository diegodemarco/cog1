namespace cog1.Literals
{
    public class ModbusLiteralsContainer : BaseLiteralsContainer
    {
        public ModbusLiteralsContainer() : base() { }
        public ModbusLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Coil { get => new ModbusLiterals.Coil().ExtractLiteral(LocaleCode); set { } }
        public virtual string DiscreteInput { get => new ModbusLiterals.DiscreteInput().ExtractLiteral(LocaleCode); set { } }
        public virtual string HoldingRegister { get => new ModbusLiterals.HoldingRegister().ExtractLiteral(LocaleCode); set { } }
        public virtual string InputRegister { get => new ModbusLiterals.InputRegister().ExtractLiteral(LocaleCode); set { } }
        public virtual string DataTypeBoolean { get => new ModbusLiterals.DataTypeBoolean().ExtractLiteral(LocaleCode); set { } }
        public virtual string TcpHost { get => new ModbusLiterals.TcpHost().ExtractLiteral(LocaleCode); set { } }
        public virtual string SlaveId { get => new ModbusLiterals.SlaveId().ExtractLiteral(LocaleCode); set { } }
        public virtual string RegisterAddress { get => new ModbusLiterals.RegisterAddress().ExtractLiteral(LocaleCode); set { } }
        public virtual string RegisterType { get => new ModbusLiterals.RegisterType().ExtractLiteral(LocaleCode); set { } }
        public virtual string DataType { get => new ModbusLiterals.DataType().ExtractLiteral(LocaleCode); set { } }
        public virtual string Registers{ get => new ModbusLiterals.Registers().ExtractLiteral(LocaleCode); set { } }
        public virtual string NewRegister { get => new ModbusLiterals.NewRegister().ExtractLiteral(LocaleCode); set { } }
        public virtual string EditRegister { get => new ModbusLiterals.EditRegister().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteRegister { get => new ModbusLiterals.DeleteRegister().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteRegisterConfirmation { get => new ModbusLiterals.DeleteRegisterConfirmation().ExtractLiteral(LocaleCode); set { } }
        public virtual string RegisterCreated { get => new ModbusLiterals.RegisterCreated().ExtractLiteral(LocaleCode); set { } }
        public virtual string RegisterUpdated { get => new ModbusLiterals.RegisterUpdated().ExtractLiteral(LocaleCode); set { } }
        public virtual string RegisterDeleted { get => new ModbusLiterals.RegisterDeleted().ExtractLiteral(LocaleCode); set { } }
        public virtual string NoRegistersToDisplay { get => new ModbusLiterals.NoRegistersToDisplay().ExtractLiteral(LocaleCode); set { } }
    }

}
