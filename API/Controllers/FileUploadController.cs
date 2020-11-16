using System;
using System.Threading.Tasks;
using API.Controllers.Models.File;
using API.Utils.Controller;
using API.Utils.Service;
using Commons.Utils;
using Core.Communication.Messages;
using Core.Communication.States;
using Core.Utils;
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

            //if (!request.IsAuthorized())
            //    return NoAuthorization();

            var uploadedFile = new UploadedFile()
            {
                FileId = FileIdGenerator.Instance.GenerateId(),
                FileName = FileUtils.RemoveIllegalCharsFromFileName(request.UploadedFile.FileName),
                ContentType = request.UploadedFile.ContentType,
                File = request.UploadedFile
            };

            var tempFileStream = await FilesService.SaveTempFileAndGetFileStream(uploadedFile);
            
            var cdnTcpConnection = Program.CdnProxyServer.GetClientConnection(1);
            
            Commons.Logger.Debug($"Client: {cdnTcpConnection != null}");
            
            cdnTcpConnection.Send(new PingMessage("Pong!").ToPacket());
            
            cdnTcpConnection.SendFile(new CdnFileState(new FileHeaderMessage()
            {
                FileId = uploadedFile.FileId,
                FileName =  uploadedFile.FileName,
                ContentType = uploadedFile.ContentType,
                Description = uploadedFile.Description,
                Created = DateTime.Now,
                LastModified = DateTime.Now, //TODO check if file was modified and not newly created 
                Version = 1.0 //TODO set version based on modification
            }, tempFileStream));
            
            return Ok(uploadedFile);
        }
        
    }
}