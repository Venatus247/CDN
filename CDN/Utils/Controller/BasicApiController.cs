using System;
using Core.Utils;
using Core.Utils.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CDN.Utils.Controller
{
    [ApiController]
    [Produces("application/json")]
    public abstract class BasicApiController<T> : Microsoft.AspNetCore.Mvc.Controller where T : Microsoft.AspNetCore.Mvc.Controller
    {
        
        protected readonly ILogger<T> Logger;
        
        public BasicApiController(ILogger<T> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Http Responses

        protected IActionResult Incomplete()
        {
            return BadRequest(new BasicResponse((int)CdnErrorCode.ClientRequestIncomplete, "Request is not complete"));
        }

        protected IActionResult NoAuthorization()
        {
            return Unauthorized(new BasicResponse((int)CdnErrorCode.ClientNotAuthorized,
                "Authorization is not valid or permission is too low"));
        }

        protected IActionResult FileNotFound()
        {
            return NotFound(new BasicResponse((int)CdnErrorCode.FileNotFound, "Requested file was not found"));
        }
        
        #endregion
        
        
        
    }
}