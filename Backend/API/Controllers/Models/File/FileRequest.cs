using Core.Data.Account;
using Core.Utils.Request;

namespace API.Controllers.Models.File
{
    public class FileRequest : AuthoritativeRequest
    {
        public bool DisableRedirect { get; set; } = false;
        public string FileId { get; set; }
        public override AccessLevel RequiredAccessLevel { get; protected set; } = AccessLevel.Public;

        public override bool IsComplete()
        {
            return FileId != null;
        }

    }
}