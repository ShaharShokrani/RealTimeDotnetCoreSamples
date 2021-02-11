using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RealTimeDotnetCoreSamples.Models;
using RealTimeDotnetCoreSamples.Models.Typicode;
using RealTimeDotnetCoreSamples.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTimeDotnetCoreSamples.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TypicodeController : ControllerBase
    {
        private readonly ILogger<TypicodeController> _logger;
        private readonly ITypicodeService _typicodeService;

        public TypicodeController(ILogger<TypicodeController> logger,
            ITypicodeService typicodeService)
        {
            _logger = logger;
            _typicodeService = typicodeService;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Albums(AlbumsRequest request)
        {
            try
            {
                if (request == null)
                {
                    this._logger.LogWarning("BadRequest in GetAlbums", new object[] { request, System.Diagnostics.Activity.Current?.RootId });
                    return BadRequest(ModelState);
                }                

                var albumsResult = await this._typicodeService.GetAlbums(request);

                if (!albumsResult.Success)
                    return StatusCode(500, albumsResult.Message);

                return Ok(albumsResult.Value);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Exception in GetAlbums", new object[] { request, System.Diagnostics.Activity.Current?.RootId });
                return StatusCode(500, "Server Error");
            }
        }
    }
}
