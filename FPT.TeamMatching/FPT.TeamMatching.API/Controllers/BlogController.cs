using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blogs;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Blogs;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_BLOGS)]
[ApiController]
// test authorize
[Authorize]
public class BlogController : ControllerBase
{
    private readonly IBlogService _service;

    public BlogController(IBlogService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BlogGetAllQuery query)
    {
        var msg = await _service.GetAll<BlogResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<BlogResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BlogCreateCommand request)
    {
        var msg = await _service.CreateBlog(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BlogUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<BlogResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] BlogRestoreCommand command)
    {
        var businessResult = await _service.Restore<BlogResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] BlogDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}