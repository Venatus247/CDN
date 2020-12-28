using Core.Utils.Request;
using Microsoft.AspNetCore.Http;

namespace CDN.Controllers.Models.File
{
    public class FileUploadRequest : BasicRequest
    {
        public IFormFile UploadedFile { get; set; }
        
        public override bool IsComplete()
        {
            return UploadedFile != null;
        }
    }
}