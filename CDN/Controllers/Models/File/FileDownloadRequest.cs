using Core.Data.File;
using Core.Utils.Request;

namespace CDN.Controllers.Models.File
{
    public class FileDownloadRequest : BasicRequest
    {
        public string FileId { get; set; }
        
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