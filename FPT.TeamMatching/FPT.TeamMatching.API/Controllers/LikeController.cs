using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Likes;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Likes;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_LIKES)]
[ApiController]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likeservice;

    public LikeController(ILikeService _service)
    {
        _likeservice = _service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] LikeGetAllQuery query)
    {
        var msg = await _likeservice.GetAll<LikeResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _likeservice.GetById<LikeResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LikeCreateCommand request)
    {
        var msg = await _likeservice.CreateOrUpdate<LikeResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] LikeUpdateCommand request)
    {
        var businessResult = await _likeservice.CreateOrUpdate<LikeResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] LikeRestoreCommand command)
    {
        var businessResult = await _likeservice.Restore<LikeResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] LikeDeleteCommand request)
    {
        var businessResult = await _likeservice.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpDelete("delete-by-blog-id/{blogId:guid}")]
    public async Task<IActionResult> DeleteByBlogId([FromRoute] Guid blogId)
    {
        var businessResult = await _likeservice.DeleteLikeByBlogId(blogId);

        return Ok(businessResult);
    }
}