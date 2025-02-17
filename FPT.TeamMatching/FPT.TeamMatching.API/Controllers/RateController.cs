using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Rates;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Rates;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_RATES)]
[ApiController]
public class RateController : ControllerBase
{
    private readonly ISpecialtyService _rateservice;

    public RateController(ISpecialtyService _service)
    {
        _rateservice = _service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] RateGetAllQuery query)
    {
        var msg = await _rateservice.GetAll<RateResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _rateservice.GetById<RateResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RateCreateCommand request)
    {
        var msg = await _rateservice.CreateOrUpdate<RateResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RateUpdateCommand request)
    {
        var businessResult = await _rateservice.CreateOrUpdate<RateResult>(request);
        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] RateRestoreCommand command)
    {
        var businessResult = await _rateservice.Restore<RateResult>(command);
        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] RateDeleteCommand request)
    {
        var businessResult = await _rateservice.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}