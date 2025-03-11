using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_IDEAS)]
[ApiController]
public class IdeaController : ControllerBase
{
    private readonly IIdeaService _service;


    public IdeaController(IIdeaService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] IdeaGetAllQuery query)
    {
        var msg = await _service.GetAll<IdeaResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<IdeaResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IdeaCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<IdeaResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] IdeaUpdateCommand request)
    {
        var businessResult = await _service.UpdateIdea(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] IdeaDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] IdeaRestoreCommand command)
    {
        var businessResult = await _service.Restore<IdeaResult>(command);

        return Ok(businessResult);
    }

    [HttpPost("student-create-pending")]
    public async Task<IActionResult> CreatePending([FromBody] IdeaStudentCreatePendingCommand request)
    {
        var msg = await _service.StudentCreatePending(request);
        return Ok(msg);
    }

    [HttpPost("lecturer-create-pending")]
    public async Task<IActionResult> CreatePending([FromBody] IdeaLecturerCreatePendingCommand request)
    {
        var msg = await _service.LecturerCreatePending(request);
        return Ok(msg);
    }

    [HttpGet("get-by-user-id")]
    public async Task<IActionResult> GetByUserIdLogin()
    {
        var businessResult = await _service.GetIdeasByUserId();
        return Ok(businessResult);
    }
}

