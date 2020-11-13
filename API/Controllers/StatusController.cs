using System;
using API.Result.Status;
using API.Utils.Controller;
using Core.Controller;
using Core.Data.File;
using Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class StatusController : BasicApiController<StatusController>
    {
        public StatusController(ILogger<StatusController> logger) : base(logger)
        {
        }

        [Route("status")]
        public IActionResult Status()
        {
            return Ok(new StatusResult()
            {
                Code = 200,
                Message = "Ok"
            });
        }
        
        [Route("file/add")]
        public IActionResult Add()
        {
            FileController.Instance.Collection.InsertOne(new SavedFile()
            {
                ContentType = "image/jpeg",
                Created = DateTime.Now,
                Description = "",
                FileName = "TestImage.jpeg",
                LastModified = DateTime.Now,
                Version = 1.0,
                FileId = FileIdGenerator.Instance.GenerateId()
            });
            return Ok(new StatusResult()
            {
                Code = 200,
                Message = "Ok"
            });
        }
        
    }
}