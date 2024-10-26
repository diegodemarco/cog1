namespace cog1.Literals
{
    public class SecurityLiteralsContainer : BaseLiteralsContainer
    {
        public SecurityLiteralsContainer() : base() { }
        public SecurityLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string BasicUser{ get => new SecurityLiterals.BasicUser().ExtractLiteral(LocaleCode); set { } }

        public virtual string Operator { get => new SecurityLiterals.Operator().ExtractLiteral(LocaleCode); set { } }

        public virtual string Administrator { get => new SecurityLiterals.Administrator().ExtractLiteral(LocaleCode); set { } }

        public virtual string UserRole { get => new SecurityLiterals.UserRole().ExtractLiteral(LocaleCode); set { } }

        public virtual string Security { get => new SecurityLiterals.Security().ExtractLiteral(LocaleCode); set { } }

        public virtual string Users { get => new SecurityLiterals.Users().ExtractLiteral(LocaleCode); set { } }

        public virtual string NewUser { get => new SecurityLiterals.NewUser().ExtractLiteral(LocaleCode); set { } }

        public virtual string EditUser { get => new SecurityLiterals.EditUser().ExtractLiteral(LocaleCode); set { } }

        public virtual string DeleteUser { get => new SecurityLiterals.DeleteUser().ExtractLiteral(LocaleCode); set { } }

        public virtual string DeleteUserConfirmation { get => new SecurityLiterals.DeleteUserConfirmation().ExtractLiteral(LocaleCode); set { } }

        public virtual string ChangePassword { get => new SecurityLiterals.ChangePassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string NoUsersToDisplay { get => new SecurityLiterals.NoUsersToDisplay().ExtractLiteral(LocaleCode); set { } }

        public virtual string UserCreated { get => new SecurityLiterals.UserCreated().ExtractLiteral(LocaleCode); set { } }

        public virtual string UserUpdated { get => new SecurityLiterals.UserUpdated().ExtractLiteral(LocaleCode); set { } }

        public virtual string UserDeleted { get => new SecurityLiterals.UserDeleted().ExtractLiteral(LocaleCode); set { } }
    }

}
