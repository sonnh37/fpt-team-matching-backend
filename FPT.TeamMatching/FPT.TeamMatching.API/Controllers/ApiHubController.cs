using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
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

    [HttpPost("get-similarities-project")]
    public async Task<IActionResult> GetSimilaritiesProject([FromBody]GetSimilaritiesProjectModel model)
    {
        var msg = await _service.GetSamilatiryProject(model.Context);
        return Ok(msg);
    }

    [HttpPost("get-recommend-blogs")]
    public async Task<IActionResult> GetRecommendBlog([FromBody] GetRecommendModel recommendModel)
    {
        var msg = await _service.GetRecommendBlogs(recommendModel.CandidateInput);
        return Ok(msg);
    }

    [HttpPost("get-recommend-users")]
    public async Task<IActionResult> GetRecommendUsers([FromBody] GetRecommendModel recommendModel)
    {
        var msg = await _service.GetRecommendUsers(recommendModel.CandidateInput);
        return Ok(msg);
    }
}