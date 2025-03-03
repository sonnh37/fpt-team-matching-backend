using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;
[Route(Const.API_HUBS)]
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
    // Cause: [FromForm]
    // public async Task<IActionResult> ScanCv([FromForm] IFormFile file)
    public async Task<IActionResult> ScanCv(IFormFile file)
    {
        var msg = await _service.ScanCv(file);
        return Ok(msg);
    }
}