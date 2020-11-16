using System;
using API.Result.Status;
using API.Utils.Controller;
using Core.Controller;
using Core.Data.Account;
using Core.Data.File;
using Core.Data.Session;
using Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

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
        
        [Route("user/add")]
        public IActionResult Add()
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
        
    }
}