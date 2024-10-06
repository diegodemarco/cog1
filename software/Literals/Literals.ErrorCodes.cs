namespace cog1.Literals
{
    public class ErrorCodes
    {
        public static class General
        {

            public class INVALID_MANDATORY_DATA : LiteralConstant
            {
                public override string EN => "Missing or empty information for mandatory fields.";
                public override string ES => "Falta información obligatoria, o está incompleta.";
                public override string PT => "Falta informação obrigatória, ou está incompleta.";
            }

            public class INVALID_MANDATORY_DATA_FORMAT : LiteralConstant
            {
                public override string EN => "Field \"{0}\" is mandatory";
                public override string ES => "El campo \"{0}\" es obligatorio";
                public override string PT => "O campo \"{0}\" é obrigatório";
            }

            public class EXCEPTION_UNEXPECTED : LiteralConstant
            {
                public override string EN => "An unexpected exception occurred.";
                public override string ES => "Ocurrió un error inesperado.";
                public override string PT => "Ocorreu um erro inesperado.";
            }

            public class NOT_IMPLEMENTED : LiteralConstant
            {
                public override string EN => "The requested operation is not implemented.";
                public override string ES => "La operación solicitada no está implementada";
                public override string PT => "A operação solicitada não está implementada";
            }

            public class DEPENDENCY_ERROR : LiteralConstant
            {
                public override string EN => "The requested operation cannot be completed because other entities depend on this information.";
                public override string ES => "La operación no puede completarse porque existen datos relacionados que dependen de la información actual.";
                public override string PT => "A operação não pode ser completada porque existem dados relacionados que dependem da informação atual.";
            }

            public class INVALID_PARAMETER_VALUE : LiteralConstant
            {
                public override string EN => "The specified value is not valid for this parameter.";
                public override string ES => "El valor indicado no es válido para este parámetro";
                public override string PT => "O valor indicado não é válido para este parâmetro";
            }

            public class INVALID_PARAMETER_VALUE_FORMAT : LiteralConstant
            {
                public override string EN => "Value \"{0}\" is not valid for parameter \"{1}\"";
                public override string ES => "El valor \"{0}\" no es válido para el parámetro \"{1}\"";
                public override string PT => "O valor \"{0}\" não é válido para o parâmetro \"{1}\"";
            }

            public class DUPLICATE_RECORD : LiteralConstant
            {
                public override string EN => "The item you are trying to create or edit is a duplicate of an existing one.";
                public override string ES => "El ítem que está intentando crear o editar es un duplicado de uno existente.";
                public override string PT => "O item que está tentando criar ou editar é um duplicado de outro já existente.";
            }

            public class NO_OPERATION : LiteralConstant
            {
                public override string EN => "You have not selected any operation to perform";
                public override string ES => "No se ha seleccionado ninguna operación para ejecutar.";
                public override string PT => "Não tem selecionado nenhuma operação para executar.";
            }

            public class INVALID_EMAIL_ADDRESS : LiteralConstant
            {
                public override string EN => "The format of the specified e-mail address is incorrect";
                public override string ES => "El formato de la dirección de e-mail indicada no es correcto";
                public override string PT => "O formato do endereço de e-mail indicado não é correto";
            }

            public class INVALID_MIN_LENGTH : LiteralConstant
            {
                public override string EN => "The parameter \"{0}\" must be at least {1} characters in length";
                public override string ES => "El parámetro \"{0}\" debe tener un mínimo de {1} caracteres";
                public override string PT => "O parâmetro  \"{0}\" deve ter um mínimo de {1} caracteres";
            }

            public class INVALID_MAX_LENGTH : LiteralConstant
            {
                public override string EN => "The parameter \"{0}\" cannot exceed {1} characters in length";
                public override string ES => "El parámetro \"{0}\" no debe exceder los {1} caracteres";
                public override string PT => "O parâmetro \"{0}\" não deve exceder {1} caracteres";
            }

            public class INVALID_EXACT_LENGTH : LiteralConstant
            {
                public override string EN => "The parameter \"{0}\" must be exactly {1} characters in length";
                public override string ES => "El parámetro \"{0}\" debe contener exactamente {1} caracteres";
                public override string PT => "O parâmetro \"{0}\" deve conter exatamente {1} caracteres";
            }

            public class INVALID_LESS_VALUE : LiteralConstant
            {
                public override string EN => "The parameter \"{0}\" must be less than {1}";
                public override string ES => "El parámetro \"{0}\" debe ser menor a {1}";
            }

            public class INVALID_GREATER_VALUE : LiteralConstant
            {
                public override string EN => "The parameter \"{0}\" must be greater than {1}";
                public override string ES => "El parámetro \"{0}\" debe ser mayor a {1}";
            }

            public class GENERAL_PARAMETER_UNKNOWN_ID : LiteralConstant
            {
                public override string EN => "There is no general parameter with the given ID.";
                public override string ES => "No existe un parámetro general con el ID indicado.";
                public override string PT => "Não existe um parâmetro geral com o ID indicado.";
            }

            public class INVALID_LANGUAGE_CODE : LiteralConstant
            {
                public override string EN => "The specified language code is not valid.";
                public override string ES => "El código de lenguaje especificado no es válido.";
                public override string PT => "O código de linguagem especificado não é válido.";
            }
        }

        public static class User
        {

            public class DUPLICATED_EMAIL : LiteralConstant
            {
                public override string EN => "This e-mail address is already registered, please use a different one";
                public override string ES => "El e-mail indicado ya está registrado en otra cuenta. Por favor, utilice uno diferente";
                public override string PT => "O e-mail indicado já está registrado para outra conta. Por favor, use outro diferente";
            }

            public class INVALID_LOGIN_DETAILS : LiteralConstant
            {
                public override string EN => "The specified username or password are incorrect";
                public override string ES => "El nombre de usuario o la contraseña ingresada son incorrectos.";
                public override string PT => "O nome de usuário ou a senha inseridos são incorretos.";
            }

            public class UNKNOWN_USER_ID : LiteralConstant
            {
                public override string EN => "There is no active user with the given ID";
                public override string ES => "No se pudo encontrar un usuario con el ID indicado";
                public override string PT => "Impossível achar um usuário com o ID indicado";
            }

            public class UNKNOWN_USER_NAME : LiteralConstant
            {
                public override string EN => "The specified user name is not registered";
                public override string ES => "El nombre de usuario indicado no está registrado.";
                public override string PT => "O nome de usuário indicado não está registrado.";
            }

            public class INVALID_PASSWORD : LiteralConstant
            {
                public override string EN => "Invalid password";
                public override string ES => "Contraseña inválida";
            }

            public class PASSWORD_MINIMUM_LENGTH : LiteralConstant
            {
                public override string EN => "Minimum length: {0} characters";
                public override string ES => "Longitud mínima: {0} caracteres";
            }

            public class PASSWORD_LOWER_CASE_REQUIRED : LiteralConstant
            {
                public override string EN => "At least one lower case character is required";
                public override string ES => "Se requiere al menos un caracter en minúsculas";
            }

            public class PASSWORD_UPPER_CASE_REQUIRED : LiteralConstant
            {
                public override string EN => "At least one lower upper character is required";
                public override string ES => "Se requiere al menos un caracter en mayúsculas";
            }

            public class PASSWORD_NUMBERS_REQUIRED : LiteralConstant
            {
                public override string EN => "At least one number is required";
                public override string ES => "Se requiere al menos un número";
            }

            public class PASSWORD_SYMBOLS_REQUIRED : LiteralConstant
            {
                public override string EN => "At least one symbol is required";
                public override string ES => "Se requiere al menos un símbolo";
            }

            public class NEW_PASSWORD_NOT_CHANGED : LiteralConstant
            {
                public override string EN => "The new password is the same as the current one. Please use a different password";
                public override string ES => "La nueva contraseña es igual a la actual. Por favor elija una contraseña diferente.";
                public override string PT => "A nova senha é igual à atual. Por favor escolha uma senha diferente.";
            }

            public class PASSWORD_RESET_EXPIRED_OR_INVALID_TOKEN : LiteralConstant
            {
                public override string EN => "The specified password reset link is invalid or expired";
                public override string ES => "El link de recuperación de contraseña no es válido, o ha expirado";
            }

        }

        public static class Security
        {

            public class MUST_BE_ADMIN : LiteralConstant
            {
                public override string EN => "The operation requires admin privileges.";
                public override string ES => "La operación require privilegios de administrador.";
                public override string PT => "A operação requer privilégios de administrador.";
            }

            public class INVALID_ACCESS_TOKEN : LiteralConstant
            {
                public override string EN => "Invalid access token";
                public override string ES => "Token de acceso no válido";
                public override string PT => "O token de acesso não é válido";
            }

        }

        public static class Variable
        {
            public class INVALID_VARIABLE_ID : LiteralConstant
            {
                public override string EN => "Invalid variable ID";
                public override string ES => "ID de variable no válido";
                public override string PT => "O ID da variável é inválido";
            }
        }

    }

}
