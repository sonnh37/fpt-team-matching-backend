using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;
[Microsoft.AspNetCore.Components.Route(Const.API_HUBS)]
[ApiController]
public class ApiHubsController : ControllerBase
{
    private readonly IApiHubService _service;

    public ApiHubsController(IApiHubService service)
    {
        _service = service;
    }
    
    [HttpPost("scan-cv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ScanCv([FromForm] IFormFile file)
    {
        var msg = await _service.ScanCv(file);
        return Ok(msg);
    }
}