using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifySemester;
using FPT.TeamMatching.Domain.Models.Requests.Queries.VerifySemester;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_VERIFY_SEMESTER)]
[ApiController]
[AllowAnonymous]
public class VerifySemesterController : ControllerBase
{
    private readonly IVerifySemesterService _verifySemesterService;

    public VerifySemesterController(IVerifySemesterService verifySemesterService)
    {
        _verifySemesterService = verifySemesterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] VerifySemesterGetAllQuery query)
    {
        var msg = await _verifySemesterService.GetVerifyingSemesters(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var msg = await _verifySemesterService.GetVerifyingSemester(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create(VerifySemesterCreateCommand verifySemester)
    {
        var msg = await _verifySemesterService.AddVerifySemester(verifySemester);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update(VerifySemesterUpdateCommand verifySemester)
    {
        var msg = await _verifySemesterService.UpdateVerifySemester(verifySemester);
        return Ok(msg);
    }
}