#pragma warning disable 1591

namespace cog1.Literals
{

    public static class ModbusLiterals
    {

        public class Coil : LiteralConstant
        {
            public override string EN => "Coil (digital output)";
            public override string ES => "Coil (salida digital)";
        }

        public class DiscreteInput : LiteralConstant
        {
            public override string EN => "Discrete input";
            public override string ES => "Entrada discreta";
        }

        public class HoldingRegister : LiteralConstant
        {
            public override string EN => "Holding register";
            public override string ES => "Registro de almacenamiento";
        }

        public class InputRegister : LiteralConstant
        {
            public override string EN => "Input register";
            public override string ES => "Registro de entrada";
        }

        public class DataTypeBoolean : LiteralConstant
        {
            public override string EN => "Boolean";
            public override string ES => "Booleano";
        }

        public class TcpHost : LiteralConstant
        {
            public override string EN => "TCP host";
            public override string ES => "Host TCP";
        }

        public class SlaveId : LiteralConstant
        {
            public override string EN => "Slave ID";
            public override string ES => "ID de esclavo";
        }

        public class RegisterAddress: LiteralConstant
        {
            public override string EN => "Register address";
            public override string ES => "Dirección del registro";
        }

        public class RegisterType : LiteralConstant
        {
            public override string EN => "Register type";
            public override string ES => "Tipo de registro";
        }

        public class DataType : LiteralConstant
        {
            public override string EN => "Data type";
            public override string ES => "Tipo de datos";
        }

        public class Registers : LiteralConstant
        {
            public override string EN => "Modbus registers";
            public override string ES => "Registros Modbus";
        }

        public class NewRegister : LiteralConstant
        {
            public override string EN => "New Modbus register";
            public override string ES => "Nuevo registro Modbus";
        }

        public class RegisterCreated : LiteralConstant
        {
            public override string EN => "A new Modbus register has been created";
            public override string ES => "Se ha creado un nuevo registro Modbus";
        }

        public class RegisterUpdated : LiteralConstant
        {
            public override string EN => "The Modbus register has been updated";
            public override string ES => "El registro Modbus ha sido actualizado";
        }

        public class RegisterDeleted : LiteralConstant
        {
            public override string EN => "The Modbus register has been deleted";
            public override string ES => "El registro Modbus ha sido eliminado";
        }

        public class EditRegister : LiteralConstant
        {
            public override string EN => "Edit Modbus register";
            public override string ES => "Editar registro Modbus";
        }

        public class DeleteRegister : LiteralConstant
        {
            public override string EN => "Delete Modbus register";
            public override string ES => "Eliminar registro Modbus";
        }

        public class DeleteRegisterConfirmation : LiteralConstant
        {
            public override string EN => "You are about to delete Modbus register \"{0}\". Are you sure you want to continue?";
            public override string ES => "Se está por eliminar el registro Modbus \"{0}\". ¿Está seguro de que desea continuar?";
        }

        public class NoRegistersToDisplay : LiteralConstant
        {
            public override string EN => "No Modbus registers to display";
            public override string ES => "No hay registros Modbus para mostrar";
        }


    }

}

#pragma warning restore 1591