namespace cog1.Literals
{
    public class DashboardLiteralsContainer : BaseLiteralsContainer
    {
        public DashboardLiteralsContainer() : base() { }
        public DashboardLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Dashboard { get => new DashboardLiterals.Dashboard().ExtractLiteral(LocaleCode); set { } }
        public virtual string CPU { get => new DashboardLiterals.CPU().ExtractLiteral(LocaleCode); set { } }
        public virtual string RAM { get => new DashboardLiterals.RAM().ExtractLiteral(LocaleCode); set { } }
        public virtual string Storage { get => new DashboardLiterals.Storage().ExtractLiteral(LocaleCode); set { } }
        public virtual string Temperature { get => new DashboardLiterals.Temperature().ExtractLiteral(LocaleCode); set { } }
        public virtual string Last5Minutes { get => new DashboardLiterals.Last5Minutes().ExtractLiteral(LocaleCode); set { } }
        public virtual string CPULast5Minutes { get => new DashboardLiterals.CPULast5Minutes().ExtractLiteral(LocaleCode); set { } }
    }

}
