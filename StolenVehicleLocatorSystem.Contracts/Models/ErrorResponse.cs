using System.Net;

namespace StolenVehicleLocatorSystem.Contracts.Dtos
{
    public class ErrorResponse
    {

        public ErrorResponse(Exception exception, HttpStatusCode statusCode)
        {
            Type = exception.GetType().Name;
            Message = exception.Message;
            StatusCode = statusCode;

        }
        public HttpStatusCode StatusCode { get; set; }

        public string Type { get; set; }
        public string? Message { get; set; }


    }
}
