using System;
using System.Net;

namespace cog1.Exceptions
{
    public class ControllerException : Exception
    {
        public string ClassName;
        public string FaultCode;
        public string FaultData;
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;

        public class JsonControllerException
        {
            public string message { get; set; }
            public string className { get; set; }
            public string faultCode { get; set; }
            public string faultData { get; set; }
        }

        public JsonControllerException ToJson()
        {
            return new JsonControllerException()
            {
                className = ClassName,
                message = Message,
                faultCode = FaultCode,
                faultData = FaultData
            };
        }

        private static string GetExceptionMessage(ErrorCode errorCode, string extraData)
        {
            GetExceptionFields(errorCode, extraData, out string dummy, out string message, out dummy, out dummy);
            return message;
        }

        private static string GetExceptionMessage(Exception e, string localeCode)
        {
            GetExceptionFields(e, localeCode, out _, out string message, out _, out _, out _);
            return message;
        }

        private static void GetExceptionFields(ErrorCode errorCode, string extraData, out string className, out string message, out string faultCode, out string faultData)
        {
            var description = errorCode.Message;
            if (!string.IsNullOrEmpty(extraData))
                description += " " + extraData;

            className = "ControllerException";
            message = description;
            faultCode = errorCode.Code.ToString();
            faultData = extraData;
        }

        private static void GetExceptionFields(Exception e, string localeCode, out string className, out string message, out string faultCode, out string faultData, out HttpStatusCode statusCode)
        {
            if (e is ControllerException)
            {
                var se = e as ControllerException;
                className = se.GetType().Name;
                message = se.Message;
                faultCode = se.FaultCode;
                faultData = se.FaultData;
                statusCode = se.StatusCode;
            }
            else
            {
                ErrorCode err;
                var Msg = e.ToString().ToLower();
                if (Msg.Contains("conflicted with the reference constraint"))
                {
                    err = new ErrorCodes(localeCode).General.DEPENDENCY_ERROR;
                }
                else if (Msg.Contains("primary key constraint") || Msg.Contains("duplicate key"))
                {
                    err = new ErrorCodes(localeCode).General.DUPLICATE_RECORD;
                }
                else
                {
                    err = new ErrorCodes(localeCode).General.EXCEPTION_UNEXPECTED;
                }
                className = e.GetType().Name;
                message = err.Message;
                faultCode = err.Code.ToString();
                faultData = e.ToString();
                statusCode = err.StatusCode;
            }
        }

        // Constructors

        public ControllerException(string faultMessage, string faultCode, string faultData) : base(faultMessage)
        {
            ClassName = "ControllerException";
            FaultCode = faultCode;
            FaultData = faultData;
        }

        public ControllerException(string faultMessage, int faultCode, string faultData) : this(faultMessage, faultCode.ToString(), faultData)
        {
        }

        public ControllerException(Exception e, string localeCode) : base(GetExceptionMessage(e, localeCode))
        {
            GetExceptionFields(e, localeCode, out ClassName, out string dummy, out FaultCode, out FaultData, out var sc);
            StatusCode = sc;
        }

        public ControllerException(Exception e) : this(e, "")
        {
        }

        public ControllerException(ErrorCode errorCode, string extraData) : base(GetExceptionMessage(errorCode, extraData))
        {
            GetExceptionFields(errorCode, extraData, out ClassName, out string dummy, out FaultCode, out FaultData);
            StatusCode = errorCode.StatusCode;
        }

        public ControllerException(ErrorCode errorCode) : this(errorCode, "")
        {
        }

    }
}

#pragma warning restore 1591
