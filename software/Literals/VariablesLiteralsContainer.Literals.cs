#pragma warning disable 1591

namespace cog1.Literals
{

    public static class VariablesLiterals
    {

        public class Variable : LiteralConstant
        {
            public override string EN => "Variable";
            public override string ES => "Variable";
        }

        public class Variables : LiteralConstant
        {
            public override string EN => "Variables";
            public override string ES => "Variables";
        }

        public class NewVariable : LiteralConstant
        {
            public override string EN => "New variable";
            public override string ES => "Nueva variable";
        }

        public class EditVariable : LiteralConstant
        {
            public override string EN => "Edit variable";
            public override string ES => "Editar variable";
        }

        public class DeleteVariable : LiteralConstant
        {
            public override string EN => "Delete variable";
            public override string ES => "Eliminar variable";
        }

        public class DeleteVariableConfirmation : LiteralConstant
        {
            public override string EN => "You are about to delete variable \"{0}\". Are you sure you want to continue?";
            public override string ES => "Se está por eliminar la variable \"{0}\". ¿Está seguro de que desea continuar?";
        }

        public class VariableType : LiteralConstant
        {
            public override string EN => "Type";
            public override string ES => "Tipo";
        }

        public class VariableId : LiteralConstant
        {
            public override string EN => "ID";
            public override string ES => "ID";
        }

        public class VariableDirection : LiteralConstant
        {
            public override string EN => "Direction";
            public override string ES => "Dirección";
        }

        public class VariableCode : LiteralConstant
        {
            public override string EN => "Code";
            public override string ES => "Código";
        }

        public class VariableUnits : LiteralConstant
        {
            public override string EN => "Units";
            public override string ES => "Unidades";
        }

        public class Binary : LiteralConstant
        {
            public override string EN => "Binary";
            public override string ES => "Binario";
        }

        public class Integer : LiteralConstant
        {
            public override string EN => "Integer";
            public override string ES => "Entero";
        }

        public class FloatingPoint : LiteralConstant
        {
            public override string EN => "Floating point";
            public override string ES => "Punto flotante";
        }

        public class Input : LiteralConstant
        {
            public override string EN => "Input";
            public override string ES => "Entrada";
        }

        public class Output : LiteralConstant
        {
            public override string EN => "Output";
            public override string ES => "Salida";
        }

        public class NoVariablesToDisplay: LiteralConstant
        {
            public override string EN => "No variables to display";
            public override string ES => "No hay variables para mostrar";
        }

        public class VariableCreated : LiteralConstant
        {
            public override string EN => "A new variable has been created";
            public override string ES => "Se ha creado una nueva variable";
        }

        public class VariableUpdated : LiteralConstant
        {
            public override string EN => "The variable has been updated";
            public override string ES => "La variable ha sido actualizada";
        }

        public class VariableDeleted : LiteralConstant
        {
            public override string EN => "The variable has been deleted";
            public override string ES => "La variable ha sido eliminada";
        }

    }

}

#pragma warning restore 1591