using System.Threading.Tasks;
using API.Utils.Controller;
using Core.Data.Account;
using Core.Utils.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;

namespace API.Controllers
{
    public class CloudController : BasicApiController<CloudController>
    {
        public CloudController(ILogger<CloudController> logger) : base(logger)
        {
        }

        [HttpPost]
        [Route("cloud/debug/test")]
        public async Task<IActionResult> AddFileReference(DebugRequest request)
        {
            if (!request.IsComplete())
                return Incomplete();
            
            request.SessionData.UserAgent = Request.Headers[HeaderNames.UserAgent];
            if (!request.IsAuthorized())
                return NoAuthorization();

            AccountController.Instance.DebugAddFileToCloud(request.Account);
            
            return Ok(request.Account.CloudFolder);

        }
        
        public class DebugRequest : AuthoritativeRequest
        {
            public override AccessLevel RequiredAccessLevel { get; protected set; } = AccessLevel.User;
        }
        
    }
}