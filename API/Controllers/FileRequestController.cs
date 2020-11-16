using System.Threading.Tasks;
using API.Controllers.Models.File;
using API.Utils.Controller;
using Core.Data.Account;
using Core.Data.Cdn;
using Core.Data.File;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;

namespace API.Controllers
{
    public class FileRequestController : BasicApiController<FileRequestController>
    {
        public FileRequestController(ILogger<FileRequestController> logger) : base(logger)
        {
        }
        
        [HttpGet]
        [Route("file/request")]
        public async Task<IActionResult> RequestFileInfo(FileRequest request)
        {
            if (!request.IsComplete())
                return Incomplete();

            var savedFile = await FileController.Instance.TryGetFile(new SearchedFile()
            {
                FileId = request.FileId
            });

            if (savedFile == null)
            {
                return FileNotFound();
            }

            if (savedFile.AccessLevel > AccessLevel.Public)
            {
                request.SessionData.UserAgent = Request.Headers[HeaderNames.UserAgent];
                if (!request.IsAuthorized() || !savedFile.GrantedAccounts.Contains(new AccountReference(request.Account.Id)))
                {
                    return Unauthorized(request);
                }
            }

            var cdnReference = savedFile.GetRandomCdnFileReference();
            if (cdnReference == null)
            {
                return Ok("File currently not saved on any cdn. Come back later");
            }
            var cursor = await CdnController.Instance.Collection.FindAsync(x => x.CdnId.Equals(cdnReference.CdnId));
            var cdn = await cursor.FirstOrDefaultAsync();
            if (cdn == null)
            {
                //TODO
                return Ok("Error happened");
            }
            
            return Ok(new FileLocationResult(cdn.GetFileUrl(savedFile)));
        }
        
    }
}