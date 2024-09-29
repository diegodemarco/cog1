using cog1.Literals;
using System.Net;

namespace cog1.Exceptions
{

    public class ErrorCode
    {
        public ErrorCode(int code, string localeCode, LiteralConstant messages, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : this(code, messages.ExtractLiteral(localeCode), statusCode)
        {
        }

        public ErrorCode(int code, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
        }

        public string Message { get; }
        public int Code { get; }
        public HttpStatusCode StatusCode { get; }
    }

}

#pragma warning restore 1591
