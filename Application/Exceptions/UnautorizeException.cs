

using System.Net;

namespace Application.Exceptions
{
    public class UnautorizeException : Exception
    {
        public List<string> ErrorMessages { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public UnautorizeException(HttpStatusCode statusCode = HttpStatusCode.Unauthorized, List<string> errorMessages = default)
        {
            StatusCode = statusCode;
            ErrorMessages = errorMessages;
        }
    }
}
