using System;
using System.Threading.Tasks;
using CDN.Controllers.Models.File;
using CDN.Utils.Controller;
using CDN.Utils.Service;
using Core.Data.Account;
using Core.Data.File;
using Core.Data.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace CDN.Controllers
{
    public class FileController : BasicApiController<FileController>
    {
        public FileController(ILogger<FileController> logger) : base(logger)
        {
        }
        /*
        [Route("user/add")]
        public IActionResult AddAccount()
        {
            var account = new Account()
            {
                AccessLevel = AccessLevel.Admin,
                AddressHistory = {new MailAddress()
                {
                    Address = "alexander.wrede@venatus247.de",
                    AddTime = DateTime.Now,
                    RemoveTime = null,
                    VerificationTime = DateTime.Now,
                    Verified = true
                }},
                AuthorizedClients = { new SessionData()
                {
                    SessionToken = "12345",
                    UserAgent = Request.Headers[HeaderNames.UserAgent]
                }},
                CurrentAddress = new MailAddress()
                {
                    Address = "alexander.wrede@venatus247.de",
                    AddTime = DateTime.Now,
                    RemoveTime = null,
                    VerificationTime = DateTime.Now,
                    Verified = true
                }
            };

            AccountController.Instance.Collection.InsertOne(account);
            
            return Ok(account);
        }*/
        
        [HttpGet]
        [Route("file/get/{fileId}/{fileName}")]
        public async Task<object> Download(string fileId, string fileName)
        {
            var request = new FileDownloadRequest()
            {
                FileId = fileId,
                FileName = fileName,
                SessionData = new SessionData()
            };
            
            if (!request.IsComplete())
                return Incomplete();
            
            var savedFile = await Core.Data.File.FileController.Instance.TryGetFile(request.CreateFileInfo());

            if (savedFile == null)
                return FileNotFound();

            if (savedFile.AccessLevel > AccessLevel.Public)
            {
                if (request.SessionData.SessionToken == null && Request.Cookies.ContainsKey("SessionToken"))
                    request.SessionData.SessionToken = Request.Cookies["SessionToken"];
                    
                request.SessionData.UserAgent = Request.Headers[HeaderNames.UserAgent];
                if (!request.IsAuthorized())
                    return NoAuthorization();

                if (!savedFile.FileOwner.AccountId.Equals(request.Account.Id) &&
                    !savedFile.GrantedAccounts.Contains(new AccountReference(request.Account.Id)))
                {
                    return NoAuthorization();
                }
            }

            return new FileStreamResult(FilesService.GetFileStream(savedFile), savedFile.ContentType)
            {
                LastModified = savedFile.LastModified,
                EnableRangeProcessing = true
            };
            return new PhysicalFileResult(savedFile.GetPath(), savedFile.ContentType)
            {
                LastModified = savedFile.LastModified,
                EnableRangeProcessing = true
            };
        }
        
    }
}