using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_IDEAS)]
[ApiController]
[Authorize]
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
    
    [HttpGet("supervisors")]
    public async Task<IActionResult> GetIdeasOfSupervisors([FromQuery] IdeaGetListOfSupervisorsQuery query)
    {
        var msg = await _service.GetIdeasOfSupervisors<IdeaResult>(query);
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

    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] IdeaUpdateStatusCommand request)
    {
        var businessResult = await _service.UpdateStatusIdea(request);

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

    [HttpPost("create-pending-by-student")]
    public async Task<IActionResult> CreatePendingByStudent([FromBody] IdeaStudentCreatePendingCommand request)
    {
        var msg = await _service.CreatePendingByStudent(request);
        return Ok(msg);
    }

    [HttpPost("create-pending-by-lecturer")]
    public async Task<IActionResult> CreatePendingByLecturer([FromBody] IdeaLecturerCreatePendingCommand request)
    {
        var msg = await _service.CreatePendingByLecturer(request);
        return Ok(msg);
    }
    
    [HttpGet("me/by-status-and-roles")]
    public async Task<IActionResult> GetIdeaVersionRequestsCurrentByStatusAndRoles([FromQuery] IdeaGetListByStatusAndRoleQuery query)
    {
        var msg = await _service.GetIdeasOfReviewerByRolesAndStatus<IdeaResult>(query);
        return Ok(msg);
    }

    [HttpGet("get-by-user-id")]
    public async Task<IActionResult> GetByUserIdLogin()
    {
        var businessResult = await _service.GetIdeasByUserId();
        return Ok(businessResult);
    }
    
    [HttpGet("me/by-status")]
    public async Task<IActionResult> GetUserIdeasByStatus([FromQuery] IdeaGetListForUserByStatus request)
    {
        var businessResult = await _service.GetUserIdeasByStatus(request);
        return Ok(businessResult);
    }
}

