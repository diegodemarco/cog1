using cog1.Literals;
using System;
using System.Net;

namespace cog1.Exceptions
{

    public class ErrorCodesContainer
    {
        protected string _LocaleCode;

        public ErrorCodesContainer(string localeCode)
        {
            _LocaleCode = localeCode;
        }
    }

    public class GeneralErrorCodes : ErrorCodesContainer         // Range 100,000 - 100,999
    {
        public GeneralErrorCodes(string localeCode) : base(localeCode) { }

        public ErrorCode INVALID_MANDATORY_DATA()
            => new ErrorCode(100000, _LocaleCode, new Literals.ErrorCodes.General.INVALID_MANDATORY_DATA(), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_MANDATORY_DATA(string fieldName)
            => new ErrorCode(100000, new Literals.ErrorCodes.General.INVALID_MANDATORY_DATA_FORMAT().Format(_LocaleCode, fieldName), HttpStatusCode.BadRequest);
        public ErrorCode EXCEPTION_UNEXPECTED
            => new ErrorCode(100001, _LocaleCode, new Literals.ErrorCodes.General.EXCEPTION_UNEXPECTED());
        public ErrorCode NOT_IMPLEMENTED
            => new ErrorCode(100002, _LocaleCode, new Literals.ErrorCodes.General.NOT_IMPLEMENTED());
        public ErrorCode DEPENDENCY_ERROR
            => new ErrorCode(100003, _LocaleCode, new Literals.ErrorCodes.General.DEPENDENCY_ERROR(), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_PARAMETER_VALUE()
            => new ErrorCode(100004, _LocaleCode, new Literals.ErrorCodes.General.INVALID_PARAMETER_VALUE(), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_PARAMETER_VALUE(LiteralConstant message)
            => new ErrorCode(100004, _LocaleCode, message, HttpStatusCode.BadRequest);
        public ErrorCode INVALID_PARAMETER_VALUE(string parameterName, string value)
            => new ErrorCode(100004, new Literals.ErrorCodes.General.INVALID_PARAMETER_VALUE_FORMAT().Format(_LocaleCode,
                    (value == null) ? "null" : value, parameterName), HttpStatusCode.BadRequest);
        public ErrorCode DUPLICATE_RECORD
            => new ErrorCode(100005, _LocaleCode, new Literals.ErrorCodes.General.DUPLICATE_RECORD(), HttpStatusCode.Conflict);
        public ErrorCode INVALID_LANGUAGE_CODE
            => new ErrorCode(100006, _LocaleCode, new Literals.ErrorCodes.General.INVALID_LANGUAGE_CODE(), HttpStatusCode.NotFound);
        public ErrorCode INVALID_OPERATION(string operation)
            => new ErrorCode(100007, operation, HttpStatusCode.BadRequest);
        public ErrorCode NO_OPERATION
            => new ErrorCode(100007, _LocaleCode, new Literals.ErrorCodes.General.NO_OPERATION(), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_EMAIL_ADDRESS
            => new ErrorCode(100008, _LocaleCode, new Literals.ErrorCodes.General.INVALID_EMAIL_ADDRESS(), HttpStatusCode.BadRequest);
        public ErrorCode GENERAL_PARAMETER_UNKNOWN_ID
            => new ErrorCode(100009, _LocaleCode, new Literals.ErrorCodes.General.GENERAL_PARAMETER_UNKNOWN_ID(), HttpStatusCode.NotFound);
        public ErrorCode CUSTOM(string message)
            => new ErrorCode(100010, message);
        public ErrorCode INVALID_MIN_LENGTH(string parameterName, int value)
            => new ErrorCode(100013, string.Format
                (
                new Literals.ErrorCodes.General.INVALID_MIN_LENGTH().ExtractLiteral(_LocaleCode),
                parameterName, value
                ), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_MAX_LENGTH(string parameterName, int value)
            => new ErrorCode(100014, string.Format
                (
                new Literals.ErrorCodes.General.INVALID_MAX_LENGTH().ExtractLiteral(_LocaleCode),
                parameterName, value
                ), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_EXACT_LENGTH(string parameterName, int value)
            => new ErrorCode(100014, string.Format
                (
                new Literals.ErrorCodes.General.INVALID_EXACT_LENGTH().ExtractLiteral(_LocaleCode),
                parameterName, value
                ), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_LESS_VALUE(string parameterName, string value)
            => new ErrorCode(100016, string.Format
                (
                new Literals.ErrorCodes.General.INVALID_LESS_VALUE().ExtractLiteral(_LocaleCode),
                parameterName, value
                ), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_GREATER_VALUE(string parameterName, string value)
            => new ErrorCode(100017, string.Format
                (
                new Literals.ErrorCodes.General.INVALID_GREATER_VALUE().ExtractLiteral(_LocaleCode),
                parameterName, value
                ), HttpStatusCode.BadRequest);
    }

    public class SecurityErrorCodes : ErrorCodesContainer                // Range 1000 - 1999
    {
        public SecurityErrorCodes(string localeCode) : base(localeCode) { }

        public ErrorCode INVALID_ACCESS_TOKEN
            => new ErrorCode(1000, _LocaleCode, new Literals.ErrorCodes.Security.INVALID_ACCESS_TOKEN(), HttpStatusCode.Unauthorized);
        public ErrorCode MUST_BE_ADMIN
            => new ErrorCode(1001, _LocaleCode, new Literals.ErrorCodes.Security.MUST_BE_ADMIN(), HttpStatusCode.Unauthorized);
        public ErrorCode MUST_BE_OPERATOR
            => new ErrorCode(1002, _LocaleCode, new Literals.ErrorCodes.Security.MUST_BE_OPERATOR(), HttpStatusCode.Unauthorized);
    }

    public class UsersErrorCodes : ErrorCodesContainer                // Range 2000 - 2999
    {
        public UsersErrorCodes(string localeCode) : base(localeCode) { }

        public ErrorCode DUPLICATE_USERNAME
            => new ErrorCode(2000, _LocaleCode, new Literals.ErrorCodes.User.DUPLICATE_USERNAME(), HttpStatusCode.Conflict);
        public ErrorCode INVALID_LOGIN_DETAILS
            => new ErrorCode(2001, _LocaleCode, new Literals.ErrorCodes.User.INVALID_LOGIN_DETAILS(), HttpStatusCode.Unauthorized);
        public ErrorCode UNKNOWN_USER_ID
            => new ErrorCode(2002, _LocaleCode, new Literals.ErrorCodes.User.UNKNOWN_USER_ID(), HttpStatusCode.NotFound);
        public ErrorCode UNKNOWN_USER_NAME
            => new ErrorCode(2003, _LocaleCode, new Literals.ErrorCodes.User.UNKNOWN_USER_NAME(), HttpStatusCode.NotFound);
        public ErrorCode CANNOT_DELETE_USER
            => new ErrorCode(2004, _LocaleCode, new Literals.ErrorCodes.User.CANNOT_DELETE_USER(), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_PASSWORD(int minLength, bool useLowerCase, bool useUpperCase, bool useNumbers, bool useSymbols)
        {
            var msg = new Literals.ErrorCodes.User.INVALID_PASSWORD().ExtractLiteral(_LocaleCode);
            if (minLength > 0)
                msg += Environment.NewLine + "- " + new Literals.ErrorCodes.User.PASSWORD_MINIMUM_LENGTH().Format(_LocaleCode, minLength);
            if (useLowerCase)
                msg += Environment.NewLine + "- " + new Literals.ErrorCodes.User.PASSWORD_LOWER_CASE_REQUIRED().ExtractLiteral(_LocaleCode);
            if (useUpperCase)
                msg += Environment.NewLine + "- " + new Literals.ErrorCodes.User.PASSWORD_UPPER_CASE_REQUIRED().ExtractLiteral(_LocaleCode);
            if (useNumbers)
                msg += Environment.NewLine + "- " + new Literals.ErrorCodes.User.PASSWORD_NUMBERS_REQUIRED().ExtractLiteral(_LocaleCode);
            if (useSymbols)
                msg += Environment.NewLine + "- " + new Literals.ErrorCodes.User.PASSWORD_SYMBOLS_REQUIRED().ExtractLiteral(_LocaleCode);
            return new ErrorCode(2006, msg, HttpStatusCode.BadRequest);
        }
        public ErrorCode NEW_PASSWORD_NOT_CHANGED
            => new ErrorCode(2009, _LocaleCode, new Literals.ErrorCodes.User.NEW_PASSWORD_NOT_CHANGED(), HttpStatusCode.BadRequest);
        public ErrorCode PASSWORD_RESET_EXPIRED_OR_INVALID_TOKEN
            => new ErrorCode(2014, _LocaleCode, new Literals.ErrorCodes.User.PASSWORD_RESET_EXPIRED_OR_INVALID_TOKEN());
    }

    public class VariablesErrorCodes : ErrorCodesContainer                // Range 3000-2999
    {
        public VariablesErrorCodes(string localeCode) : base(localeCode) { }

        public ErrorCode INVALID_VARIABLE_ID
            => new ErrorCode(3000, _LocaleCode, new Literals.ErrorCodes.Variable.INVALID_VARIABLE_ID(), HttpStatusCode.NotFound);
        public ErrorCode VARIABLE_NOT_WRITABLE
            => new ErrorCode(3001, _LocaleCode, new Literals.ErrorCodes.Variable.VARIABLE_NOT_WRITABLE(), HttpStatusCode.BadRequest);
        public ErrorCode INVALID_VARIABLE_CODE
            => new ErrorCode(3002, _LocaleCode, new Literals.ErrorCodes.Variable.INVALID_VARIABLE_CODE(), HttpStatusCode.NotFound);
        public ErrorCode DUPLICATE_VARIABLE_CODE
            => new ErrorCode(3003, _LocaleCode, new Literals.ErrorCodes.Variable.DUPLICATE_VARIABLE_CODE(), HttpStatusCode.Conflict);
    }

    public class ModbusErrorCodes : ErrorCodesContainer                // Range 4000-4999
    {
        public ModbusErrorCodes(string localeCode) : base(localeCode) { }

        public ErrorCode INVALID_REGISTER_ID
            => new ErrorCode(4000, _LocaleCode, new Literals.ErrorCodes.Modbus.INVALID_REGISTER_ID(), HttpStatusCode.NotFound);
        public ErrorCode INVALID_DATA_TYPE_FOR_REGISTER_TYPE
            => new ErrorCode(4001, _LocaleCode, new Literals.ErrorCodes.Modbus.INVALID_DATA_TYPE_FOR_REGISTER_TYPE(), HttpStatusCode.NotFound);
        public ErrorCode COULD_NOT_WRITE_REGISTER(string errorMessage)
            => new ErrorCode(4002, new Literals.ErrorCodes.Modbus.COULD_NOT_WRITE_REGISTER().Format(_LocaleCode, errorMessage), HttpStatusCode.NotFound);
    }

    public class NetworkErrorCodes : ErrorCodesContainer                // Range 5000-5999
    {
        public NetworkErrorCodes(string localeCode) : base(localeCode) { }

        public ErrorCode WIFI_CONNECT_ERROR
            => new ErrorCode(5000, _LocaleCode, new Literals.ErrorCodes.Network.WIFI_CONNECT_ERROR(), HttpStatusCode.NotFound);
        public ErrorCode WIFI_DISCONNECT_ERROR
            => new ErrorCode(5001, _LocaleCode, new Literals.ErrorCodes.Network.WIFI_DISCONNECT_ERROR(), HttpStatusCode.NotFound);
        public ErrorCode WIFI_FORGET_ERROR
            => new ErrorCode(5002, _LocaleCode, new Literals.ErrorCodes.Network.WIFI_FORGET_ERROR(), HttpStatusCode.NotFound);
        public ErrorCode IP_CONFIG_READ_ERROR
            => new ErrorCode(5003, _LocaleCode, new Literals.ErrorCodes.Network.IP_CONFIG_READ_ERROR(), HttpStatusCode.NotFound);
        public ErrorCode IP_CONFIG_ERROR
            => new ErrorCode(5004, _LocaleCode, new Literals.ErrorCodes.Network.IP_CONFIG_ERROR(), HttpStatusCode.NotFound);
        public ErrorCode LINK_CONFIG_READ_ERROR
            => new ErrorCode(5005, _LocaleCode, new Literals.ErrorCodes.Network.LINK_CONFIG_READ_ERROR(), HttpStatusCode.NotFound);
        public ErrorCode LINK_CONFIG_ERROR
            => new ErrorCode(5006, _LocaleCode, new Literals.ErrorCodes.Network.LINK_CONFIG_ERROR(), HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Error codes returned by the business layer
    /// </summary>
    public class ErrorCodes
    {
        protected string localeCode = "en";
        public string LocaleCode => localeCode;

        readonly Lazy<GeneralErrorCodes> _generalErrorCodes;
        readonly Lazy<UsersErrorCodes> _usersErrorCodes;
        readonly Lazy<SecurityErrorCodes> _securityErrorCodes;
        readonly Lazy<VariablesErrorCodes> _variablesErrorCodes;
        readonly Lazy<ModbusErrorCodes> _modbusErrorCodes;
        readonly Lazy<NetworkErrorCodes> _networkErrorCodes;

        public ErrorCodes(string localeCode)
        {
            this.localeCode = localeCode;
            _generalErrorCodes = new (() => new GeneralErrorCodes(localeCode));
            _usersErrorCodes = new(() => new UsersErrorCodes(localeCode));
            _securityErrorCodes = new(() => new SecurityErrorCodes(localeCode));
            _variablesErrorCodes = new(() => new VariablesErrorCodes(localeCode));
            _modbusErrorCodes = new(() => new ModbusErrorCodes(localeCode));
            _networkErrorCodes = new(() => new NetworkErrorCodes(localeCode));
        }

        public GeneralErrorCodes General => _generalErrorCodes.Value;
        public UsersErrorCodes Users => _usersErrorCodes.Value;
        public SecurityErrorCodes Security => _securityErrorCodes.Value;
        public VariablesErrorCodes Variables => _variablesErrorCodes.Value;
        public ModbusErrorCodes Modbus => _modbusErrorCodes.Value;
        public NetworkErrorCodes Network => _networkErrorCodes.Value;
    }
}

#pragma warning restore 1591
