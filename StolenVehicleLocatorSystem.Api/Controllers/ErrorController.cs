using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StolenVehicleLocatorSystem.Contracts.Dtos;
using StolenVehicleLocatorSystem.Contracts.Exceptions;
using System.Net;

namespace StolenVehicleLocatorSystem.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            var code = HttpStatusCode.InternalServerError; // Internal Server Error by default

            if (exception is BadRequestException)
                code = HttpStatusCode.BadRequest;
            else if (exception is HttpStatusException httpException)
            {
                code = httpException.Status;
            }

            Response.StatusCode = (int)code;

            return new ErrorResponse(exception, code);
        }
    }
}
