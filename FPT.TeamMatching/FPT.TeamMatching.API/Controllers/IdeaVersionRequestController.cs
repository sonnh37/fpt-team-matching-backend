using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_IDEA_VERSION_REQUESTS)]
[ApiController]
[Authorize]
public class IdeaVersionRequestController : ControllerBase
{
    private readonly IIdeaVersionRequestService _service;


    public IdeaVersionRequestController(IIdeaVersionRequestService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] IdeaVersionRequestGetAllQuery query)
    {
        var msg = await _service.GetAll<IdeaVersionRequestResult>(query);
        return Ok(msg);
    }
    
    [HttpGet("me/by-status-and-roles")]
    public async Task<IActionResult> GetIdeaVersionRequestsCurrentByStatusAndRoles([FromQuery] IdeaGetListByStatusAndRoleQuery query)
    {
        var msg = await _service.GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus<IdeaVersionRequestResult>(query);
        return Ok(msg);
    }
    
    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] IdeaVersionRequestUpdateStatusCommand request)
    {
        var businessResult = await _service.UpdateStatus(request);

        return Ok(businessResult);
    }
    
    [HttpPost("create-council-requests")]
    public async Task<IActionResult> CreateCouncilRequests([FromBody] IdeaVersionRequestCreateForCouncilsCommand command)
    {
        var result = await _service.CreateCouncilRequestsForIdea(command);
        return Ok(result);
    }
    
    [HttpGet("without-reviewer")]
    public async Task<IActionResult> GetAllUnassignedReviewer([FromQuery] GetQueryableQuery query)
    {
        var msg = await _service.GetAllUnassignedReviewer<IdeaVersionRequestResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<IdeaVersionRequestResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IdeaVersionRequestCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<IdeaVersionRequestResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] IdeaVersionRequestUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<IdeaVersionRequestResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] IdeaVersionRequestDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] IdeaVersionRequestRestoreCommand command)
    {
        var businessResult = await _service.Restore<IdeaVersionRequestResult>(command);

        return Ok(businessResult);
    }

    [HttpPut("respond-by-mentor-or-council")]
    public async Task<IActionResult> RespondByMentorOrCouncil([FromBody] IdeaVersionRequestLecturerOrCouncilResponseCommand request)
    {
        var businessResult = await _service.RespondByMentorOrCouncil(request);

        return Ok(businessResult);
    }

    //[HttpPut("council-response")]
    //public async Task<IActionResult> CouncilResponse([FromBody] IdeaVersionRequestLecturerOrCouncilResponseCommand request)
    //{
    //    var businessResult = await _service.CouncilResponse(request);

    //    return Ok(businessResult);
    //}
}
