namespace cog1.Literals
{
    public class LoggingLiteralsContainer : BaseLiteralsContainer
    {
        public LoggingLiteralsContainer() : base() { }
        public LoggingLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Logs { get => new LoggingLiterals.Logs().ExtractLiteral(LocaleCode); set { } }
        public virtual string Category { get => new LoggingLiterals.Category().ExtractLiteral(LocaleCode); set { } }
        public virtual string Level { get => new LoggingLiterals.Level().ExtractLiteral(LocaleCode); set { } }
        public virtual string AllCategories { get => new LoggingLiterals.AllCategories().ExtractLiteral(LocaleCode); set { } }
        public virtual string AllLevels { get => new LoggingLiterals.AllLevels().ExtractLiteral(LocaleCode); set { } }
        public virtual string Timestamp { get => new LoggingLiterals.Timestamp().ExtractLiteral(LocaleCode); set { } }
        public virtual string Message { get => new LoggingLiterals.Message().ExtractLiteral(LocaleCode); set { } }
        public virtual string NoLogEntriesToDisplay { get => new LoggingLiterals.NoLogEntriesToDisplay().ExtractLiteral(LocaleCode); set { } }
        public virtual string Information { get => new LoggingLiterals.Information().ExtractLiteral(LocaleCode); set { } }
        public virtual string Warning { get => new LoggingLiterals.Warning().ExtractLiteral(LocaleCode); set { } }
        public virtual string Error { get => new LoggingLiterals.Error().ExtractLiteral(LocaleCode); set { } }
        public virtual string General { get => new LoggingLiterals.General().ExtractLiteral(LocaleCode); set { } }
        public virtual string Modbus { get => new LoggingLiterals.Modbus().ExtractLiteral(LocaleCode); set { } }
        public virtual string Variables { get => new LoggingLiterals.Variables().ExtractLiteral(LocaleCode); set { } }
        public virtual string Security { get => new LoggingLiterals.Security().ExtractLiteral(LocaleCode); set { } }
        public virtual string Integrations { get => new LoggingLiterals.Integrations().ExtractLiteral(LocaleCode); set { } }
        public virtual string System { get => new LoggingLiterals.System().ExtractLiteral(LocaleCode); set { } }
    }

}
