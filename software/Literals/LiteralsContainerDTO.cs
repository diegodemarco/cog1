namespace cog1.Literals
{
    public class LiteralsContainerDTO : BaseLiteralsContainer
    {
        private CommonLiteralsContainer _common;
        private DashboardLiteralsContainer _dashboard;
        private SecurityLiteralsContainer _security;
        private NetworkLiteralsContainer _network;
        private VariablesLiteralsContainer _variables;
        private ModbusLiteralsContainer _modbus;

        public LiteralsContainerDTO() : base() 
        {
            Common = null;
        }

        public LiteralsContainerDTO(string localeCode) : base(localeCode) 
        { 
            
        }

        public CommonLiteralsContainer Common
        {
            get
            {
                if (_common == null)
                    _common = new CommonLiteralsContainer(LocaleCode);
                return _common;
            }
            set { }
        }

        public DashboardLiteralsContainer Dashboard
        {
            get
            {
                if (_dashboard == null)
                    _dashboard = new DashboardLiteralsContainer(LocaleCode);
                return _dashboard;
            }
            set { }
        }

        public SecurityLiteralsContainer Security
        {
            get
            {
                if (_security == null)
                    _security = new SecurityLiteralsContainer(LocaleCode);
                return _security;
            }
            set { }
        }

        public NetworkLiteralsContainer Network
        {
            get
            {
                if (_network == null)
                    _network = new NetworkLiteralsContainer(LocaleCode);
                return _network;
            }
            set { }
        }

        public VariablesLiteralsContainer Variables
        {
            get
            {
                if (_variables == null)
                    _variables = new VariablesLiteralsContainer(LocaleCode);
                return _variables;
            }
            set { }
        }

        public ModbusLiteralsContainer Modbus
        {
            get
            {
                if (_modbus == null)
                    _modbus = new ModbusLiteralsContainer(LocaleCode);
                return _modbus;
            }
            set { }
        }

    }

}