using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[ApiController]
[Route(Const.API_FILE_UPLOAD)]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    public FileUploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadCv([FromForm] FileUploadRequest request)
    {
        var businessResult = await _fileUploadService.UploadFile(request);

        return Ok(businessResult);
    }
}
