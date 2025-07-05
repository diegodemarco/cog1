#pragma warning disable 1591

namespace cog1.Literals
{

    public static class NetworkLiterals
    {

        public class Network : LiteralConstant
        {
            public override string EN => "Network";
            public override string ES => "Red";
        }

        public class Summary : LiteralConstant
        {
            public override string EN => "Summary";
            public override string ES => "Resumen";
        }

        public class NetworkSummary : LiteralConstant
        {
            public override string EN => "Network summary";
            public override string ES => "Resumen de red";
        }

        public class Connection : LiteralConstant
        {
            public override string EN => "Connection";
            public override string ES => "Conexión";
        }

        public class Status : LiteralConstant
        {
            public override string EN => "Status";
            public override string ES => "Estado";
        }

        public class Connected : LiteralConstant
        {
            public override string EN => "Connected";
            public override string ES => "Conectado";
        }

        public class Disconnected : LiteralConstant
        {
            public override string EN => "Disconnected";
            public override string ES => "Desconectado";
        }

        public class IpMethod : LiteralConstant
        {
            public override string EN => "Method";
            public override string ES => "Método";
        }

        public class IpFixed : LiteralConstant
        {
            public override string EN => "Fixed IP";
            public override string ES => "IP fijo";
        }

        public class IpAddress : LiteralConstant
        {
            public override string EN => "Address";
            public override string ES => "Dirección";
        }

        public class Gateway : LiteralConstant
        {
            public override string EN => "Gateway";
            public override string ES => "Gateway";
        }

        public class Frequency : LiteralConstant
        {
            public override string EN => "Frequency";
            public override string ES => "Frecuencia";
        }

        public class Speed : LiteralConstant
        {
            public override string EN => "Speed";
            public override string ES => "Velocidad";
        }

        public class FullDuplex : LiteralConstant
        {
            public override string EN => "Full duplex";
            public override string ES => "Full duplex";
        }

        public class HalfDuplex : LiteralConstant
        {
            public override string EN => "Half duplex";
            public override string ES => "Half duplex";
        }

        public class MacAddress : LiteralConstant
        {
            public override string EN => "MAC address";
            public override string ES => "Dirección MAC";
        }

        public class Connect : LiteralConstant
        {
            public override string EN => "Connect";
            public override string ES => "Conectar";
        }

        public class Disconnect : LiteralConstant
        {
            public override string EN => "Disconnect";
            public override string ES => "Desconectar";
        }

        public class Forget : LiteralConstant
        {
            public override string EN => "Forget";
            public override string ES => "Olvidar";
        }

        public class Scanning : LiteralConstant
        {
            public override string EN => "Scanning";
            public override string ES => "Buscando redes";
        }

        public class ScanningPleaseWait : LiteralConstant
        {
            public override string EN => "Scanning networks, please wait";
            public override string ES => "Buscando redes, por favor espere";
        }

        public class ConnectingPleaseWait : LiteralConstant
        {
            public override string EN => "Connecting, please wait";
            public override string ES => "Conectando, por favor espere";
        }

        public class DisconnectingPleaseWait : LiteralConstant
        {
            public override string EN => "Disconnecting, please wait";
            public override string ES => "Desconectando, por favor espere";
        }

        public class ForgettingPleaseWait : LiteralConstant
        {
            public override string EN => "Forgetting, please wait";
            public override string ES => "Olvidando, por favor espere";
        }

        public class ConfiguringPleaseWait : LiteralConstant
        {
            public override string EN => "Configuring, please wait";
            public override string ES => "Configurando, por favor espere";
        }

        public class IpConfiguration : LiteralConstant
        {
            public override string EN => "IP Configuration";
            public override string ES => "Configuración IP";
        }

        public class LinkConfiguration : LiteralConstant
        {
            public override string EN => "Link configuration";
            public override string ES => "Configuración del link";
        }

        public class WiFiNetworks : LiteralConstant
        {
            public override string EN => "Wi-Fi networks";
            public override string ES => "Redes Wi-Fi";
        }

        public class ConfirmChanges : LiteralConstant
        {
            public override string EN => "The connection to the gateway may be lost as a consequence of the operation you are trying to perform. " +
                "If you are configuring remotely, physical access to the gateway may be needed to regain access";
            public override string ES => "La conexión al gateway podría perderse como consecuencia de la operación que está intentando ejecutar. " +
                "Si está configurando en forma remota, es posible que se requiera acceso físico al gateway para recuperar el acceso";
        }

        public class ConfirmForget : LiteralConstant
        {
            public override string EN => "Credentials for this connection will be forgotten. If you need to reconnect to this network, you will be asked to enter the credentials again";
            public override string ES => "Se perderán las credenciales para esta conexión. Si necesita reconectarse a esta red, se le pedirán las credenciales nuevamente";
        }

        public class ConfigurationAppliedSuccessfully : LiteralConstant
        {
            public override string EN => "Configuration applied successfully";
            public override string ES => "Configuración aplicada correctamente";
        }

    }

}

#pragma warning restore 1591