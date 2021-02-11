using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RealTimeDotnetCoreSamples.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTimeDotnetCoreSamples.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IEnumerable<IUserService> _userServices;

        public UsersController(ILogger<UsersController> logger,
            IEnumerable<IUserService> userServices)
        {
            _logger = logger;
            _userServices = userServices;
        }

        [HttpGet]
        [HttpPost]
        public async Task UsersSSE()
        {
            try
            {
                HttpContext.Response.ContentType = "text/event-stream";

                ResultHandler<IEnumerable<UserModel>> usersResult;

                foreach (IUserService userService in this._userServices)
                {
                    usersResult = await userService.GetUsers(new UsersRequest());

                    if (usersResult.Success)
                    {
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(usersResult.Value));
                        await HttpContext.Response.Body.FlushAsync();
                    }
                }

                HttpContext.Response.Body.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Exception in GetUsers", new object[] { new UsersRequest(), System.Diagnostics.Activity.Current?.RootId });
                return;
            }

        }
    }
}
