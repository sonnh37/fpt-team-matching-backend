using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_INVITATIONS)]
[ApiController]
public class InvitationController : ControllerBase
{
    private readonly IInvitationService _service;

    public InvitationController(IInvitationService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] InvitationGetAllQuery query)
    {
        var msg = await _service.GetAll<InvitationResult>(query);
        return Ok(msg);
    }

    [HttpGet("get-user-invitations-by-type")]
    public async Task<IActionResult> GetUserInvitationsByType([FromQuery] InvitationGetByTypeQuery query)
    {
        var msg = await _service.GetUserInvitationsByType(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<InvitationResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InvitationCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<InvitationResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] InvitationUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<InvitationResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] InvitationDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] InvitationRestoreCommand command)
    {
        var businessResult = await _service.Restore<InvitationResult>(command);

        return Ok(businessResult);
    }

    [HttpPost("send-by-student")]
    public async Task<IActionResult> StudentCreate([FromBody] InvitationStudentCreatePendingCommand request)
    {
        var msg = await _service.StudentCreatePending(request);
        return Ok(msg);
    }

    [HttpPost("sent-by-team")]
    public async Task<IActionResult> TeamCreate([FromBody] InvitationTeamCreatePendingCommand request)
    {
        var msg = await _service.TeamCreatePending(request);
        return Ok(msg);
    }

    [HttpGet("check-if-student-sent-invitation/{projectId:guid}")]
    public async Task<IActionResult> CheckIfStudentSendInvitationByProjectId([FromRoute] Guid projectId)
    {
        var msg = await _service.CheckIfStudentSendInvitationByProjectId(projectId);
        bool hasSent = (bool)msg.Data;
        if (hasSent)
            return Ok(new
            {
                status = msg.Status,
                message = "Invitation has been sent!",
                hasSent = hasSent
            });
        return Ok(new
        {
            status = msg.Status,
            message = "Invitation has not been sent!",
            hasSent = hasSent
        });
    }
}
