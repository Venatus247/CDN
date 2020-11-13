using System;
using System.Threading.Tasks;
using CDN.Controllers.Models.File;
using CDN.Utils.Controller;
using CDN.Utils.Service;
using Commons.Utils;
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
        }

        [Route("file/upload")]
        public async Task<IActionResult> Upload([FromForm]FileUploadRequest request)
        {
            if (request == null)
                return BadRequest();
            
            if (!request.IsComplete())
                return Incomplete();
            
            var uploadedFile = new UploadedFile()
            {
                FileName = request.UploadedFile.FileName,
                ContentType = request.UploadedFile.ContentType,
                File = request.UploadedFile
            };

            var savedFile = await FilesService.SaveFile(uploadedFile);
            
            return savedFile != null ? Ok(savedFile) : Ok("Error");
        }
        
        [HttpGet]
        [Route("file/get/{fileId}/{fileName}")]
        public async Task<object> Download([FromRoute]string fileId, [FromRoute]string fileName, [FromForm]SessionData sessionData)
        {
            //TODO implement file access levels, a.e. public, restricted
            
            var savedFile = await Core.Data.File.FileController.Instance.TryGetFile(new RequestedFileInfo()
            {
                FileId = fileId
            });

            if (savedFile == null)
                return FileNotFound();

            if (savedFile.AccessLevel > AccessLevel.None)
            {
                return Ok("Missing access level");
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