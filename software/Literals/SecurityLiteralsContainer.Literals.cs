#pragma warning disable 1591

namespace cog1.Literals
{

    public static class SecurityLiterals
    {

        public class Operator : LiteralConstant
        {
            public override string EN => "Operator";
            public override string ES => "Operador";
        }

        public class BasicUser : LiteralConstant
        {
            public override string EN => "Basic user";
            public override string ES => "Usuario básico";
        }

        public class Administrator : LiteralConstant
        {
            public override string EN => "Administrator";
            public override string ES => "Administrador";
        }

        public class UserRole: LiteralConstant
        {
            public override string EN => "User role";
            public override string ES => "Rol del usuario";
        }

        public class Security : LiteralConstant
        {
            public override string EN => "Security";
            public override string ES => "Seguridad";
        }

        public class Users : LiteralConstant
        {
            public override string EN => "Users";
            public override string ES => "Usuarios";
        }

        public class NewUser : LiteralConstant
        {
            public override string EN => "New user";
            public override string ES => "Nuevo usuario";
        }

        public class EditUser : LiteralConstant
        {
            public override string EN => "Edit user";
            public override string ES => "Editar usuario";
        }

        public class DeleteUser : LiteralConstant
        {
            public override string EN => "Delete user";
            public override string ES => "Eliminar usuario";
        }

        public class DeleteUserConfirmation : LiteralConstant
        {
            public override string EN => "You are about to delete user \"{0}\". Are you sure you want to continue?";
            public override string ES => "Se está por eliminar el usuario \"{0}\". ¿Está seguro de que desea continuar?";
        }

        public class ChangePassword : LiteralConstant
        {
            public override string EN => "Change password";
            public override string ES => "Cambiar contraseña";
        }

        public class NoUsersToDisplay : LiteralConstant
        {
            public override string EN => "No users to display";
            public override string ES => "No hay usuarios para mostrar";
        }

        public class UserCreated : LiteralConstant
        {
            public override string EN => "A new user has been created";
            public override string ES => "Se ha creado un nuevo usuario";
        }

        public class UserUpdated : LiteralConstant
        {
            public override string EN => "The user has been updated";
            public override string ES => "El usuario ha sido actualizado";
        }

        public class UserDeleted : LiteralConstant
        {
            public override string EN => "The user has been deleted";
            public override string ES => "El usuario ha sido eliminado";
        }

    }

}

#pragma warning restore 1591