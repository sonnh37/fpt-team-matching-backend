using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Applications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Applications;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_APPLICATIONS)]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _jobservice;

    public ApplicationController(IApplicationService _service)
    {
        _jobservice = _service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ApplicationGetAllQuery query)
    {
        var msg = await _jobservice.GetAll<ApplicationResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _jobservice.GetById<ApplicationResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApplicationCreateCommand request)
    {
        var msg = await _jobservice.CreateOrUpdate<ApplicationResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ApplicationUpdateCommand request)
    {
        var businessResult = await _jobservice.CreateOrUpdate<ApplicationResult>(request);
        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] ApplicationRestoreCommand command)
    {
        var businessResult = await _jobservice.Restore<ApplicationResult>(command);
        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] ApplicationDeleteCommand request)
    {
        var businessResult = await _jobservice.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}