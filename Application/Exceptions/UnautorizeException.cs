

using System.Net;

namespace Application.Exceptions
{
    public class UnautorizeException : Exception
    {
        public List<string> ErrorMessages { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public UnautorizeException(List<string> errorMessages = default, HttpStatusCode statusCode = HttpStatusCode.Unauthorized)
        {
            StatusCode = statusCode;
            ErrorMessages = errorMessages;
        }
    }
}
