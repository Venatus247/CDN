using Core.Data.Account;
using Core.Data.File;
using Core.Utils.Request;

namespace CDN.Controllers.Models.File
{
    public class FileDownloadRequest : AuthoritativeRequest
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        
        public override AccessLevel RequiredAccessLevel { get; protected set; } = AccessLevel.Public;

        public override bool IsComplete()
        {
            return FileId != null;
        }

        public RequestedFileInfo CreateFileInfo()
        {
            return new RequestedFileInfo()
            {
                FileId = FileId
            };
        }
        
    }
}