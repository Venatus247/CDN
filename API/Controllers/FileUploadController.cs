using System.Threading.Tasks;
using API.Controllers.Models.File;
using API.Utils.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class FileUploadController : BasicApiController<FileUploadController>
    {
        public FileUploadController(ILogger<FileUploadController> logger) : base(logger)
        {
        }

        [Route("file/upload")]
        public async Task<IActionResult> Upload([FromForm]FileUploadRequest request)
        {

            if (!request.IsComplete())
                return Incomplete();

            if (!request.IsAuthorized())
                return NoAuthorization();
            
            
            return null;
        }
        
    }
}