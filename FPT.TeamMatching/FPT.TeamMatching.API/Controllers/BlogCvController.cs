using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.BlogCvs;
using FPT.TeamMatching.Domain.Models.Requests.Queries.BlogCvs;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_BLOGCVS)]
[ApiController]
public class BlogCvController : ControllerBase
{
    private readonly IBlogCvService _service;

    public BlogCvController(IBlogCvService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BlogCvGetAllQuery query)
    {
        var msg = await _service.GetAll<BlogCvResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<BlogCvResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BlogCvCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<BlogCvResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BlogCvUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<BlogCvResult>(request);
        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] BlogCvRestoreCommand command)
    {
        var businessResult = await _service.Restore<BlogCvResult>(command);
        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] BlogCvDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}