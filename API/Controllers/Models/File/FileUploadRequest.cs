using Core.Data.Account;
using Core.Utils.Request;
using Microsoft.AspNetCore.Http;

namespace API.Controllers.Models.File
{
    public class FileUploadRequest : AuthoritativeRequest
    {
        public IFormFile UploadedFile { get; set; }
        public override AccessLevel RequiredAccessLevel { get; protected set; } = AccessLevel.User;
    }
}