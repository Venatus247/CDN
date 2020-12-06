using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers.Models.File;
using API.Utils.Controller;
using API.Utils.Service;
using Commons.Utils;
using Communication.Messages.File;
using Communication.States;
using Core.Data.Account;
using Core.Data.File;
using Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace API.Controllers
{
    public class FileUploadController : BasicApiController<FileUploadController>
    {
        public FileUploadController(ILogger<FileUploadController> logger) : base(logger)
        {
        }

        //[RequestSizeLimit(52428800)] //50MB
        [DisableRequestSizeLimit]
        [Route("file/upload")]
        public async Task<IActionResult> Upload([FromForm]FileUploadRequest request)
        {

            if (!request.IsComplete())
                return Incomplete();

            request.SessionData.UserAgent = Request.Headers[HeaderNames.UserAgent];
            
            if (!request.IsAuthorized())
                return NoAuthorization();

            var uploadedFile = new UploadedFile()
            {
                FileId = UidGenerator.Instance.GenerateId(),
                FileName = FileUtils.RemoveIllegalCharsFromFileName(request.UploadedFile.FileName),
                ContentType = request.UploadedFile.ContentType,
                File = request.UploadedFile
            };

            var tempFileStream = await FilesService.SaveTempFileAndGetFileStream(uploadedFile);
            
            //TODO get cdn dynamically 
            var cdnTcpConnection = Program.CdnProxyServer.GetClientConnection(1);
            cdnTcpConnection.SendFile(new CdnFileState(new FileHeaderMessage()
            {
                FileId = uploadedFile.FileId,
                FileName =  uploadedFile.FileName,
                ContentType = uploadedFile.ContentType,
                Description = uploadedFile.Description,
                Created = DateTime.Now,
                LastModified = DateTime.Now, //TODO check if file was modified and not newly created 
                Version = 1.0, //TODO set version based on modification
                AccessLevel = AccessLevel.Public, //TODO set based on user's settings
                FileOwner = new AccountReference(request.Account.Id),
                GrantedAccounts = new List<AccountReference>() //TODO set based on user's settings
            }, tempFileStream));

            var savedFile = uploadedFile.ToSavedFile(request.Account);

            await FileController.Instance.Collection.InsertOneAsync(savedFile);
            
            return Ok(uploadedFile);
        }
        
    }
}