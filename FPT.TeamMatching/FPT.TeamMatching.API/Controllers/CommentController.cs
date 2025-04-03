using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comments;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Comments;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_COMMENTS)]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _Commentservice;

    public CommentController(ICommentService _service)
    {
        _Commentservice = _service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CommentGetAllQuery query)
    {
        var msg = await _Commentservice.GetAll<CommentResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _Commentservice.GetById<CommentResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentCreateCommand request)
    {
        var msg = await _Commentservice.CreateComment(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CommentUpdateCommand request)
    {
        var businessResult = await _Commentservice.CreateOrUpdate<CommentResult>(request);
        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] CommentRestoreCommand command)
    {
        var businessResult = await _Commentservice.Restore<CommentResult>(command);
        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] CommentDeleteUpdate request)
    {
        var businessResult = await _Commentservice.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}