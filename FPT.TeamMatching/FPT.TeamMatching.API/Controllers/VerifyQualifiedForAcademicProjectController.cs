using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifyQualifiedForAcademicProject;
using FPT.TeamMatching.Domain.Models.Requests.Queries.VerifyQualifiedForAcademicProject;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_VERIFY_QUALIFIED)]
[ApiController]
[AllowAnonymous]
public class VerifyQualifiedForAcademicProjectController : ControllerBase
{
    private readonly IVerifyQualifiedForAcademicProjectService _verifyQualifiedForAcademicProjectService;

    public VerifyQualifiedForAcademicProjectController(
        IVerifyQualifiedForAcademicProjectService verifyQualifiedForAcademicProjectService)
    {
        _verifyQualifiedForAcademicProjectService = verifyQualifiedForAcademicProjectService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] VerifyQualifiedForAcademicProjectGetAllQuery query)
    {
        var msg = await _verifyQualifiedForAcademicProjectService.GetAll(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromQuery] Guid id)
    {
        var msg = await _verifyQualifiedForAcademicProjectService.GetById(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] VerifyQualifiedForAcademicProjectCreateCommand model)
    {
        var msg = await _verifyQualifiedForAcademicProjectService.Add(model);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] VerifyQualifiedForAcademicProjectUpdateCommand model)
    {
        var msg = await _verifyQualifiedForAcademicProjectService.Update(model);
        return Ok(msg);
    }
}