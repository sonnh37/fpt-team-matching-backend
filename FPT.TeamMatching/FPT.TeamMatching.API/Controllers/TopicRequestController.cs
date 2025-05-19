using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_TOPIC_REQUESTS)]
[ApiController]
[Authorize]
public class TopicRequestController : ControllerBase
{
    private readonly ITopicRequestService _service;

    public TopicRequestController(ITopicRequestService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TopicRequestGetAllQuery query)
    {
        var msg = await _service.GetAll<TopicRequestResult>(query);
        return Ok(msg);
    }


    [HttpGet("by-status-different-pending")]
    public async Task<IActionResult> GetAllExceptPending([FromQuery] TopicRequestGetAllQuery query)
    {
        var msg = await _service.GetAllExceptPending<TopicRequestResult>(query);
        return Ok(msg);
    }

    [HttpGet("me/by-status-and-roles")]
    public async Task<IActionResult> GetTopicRequestsCurrentByStatusAndRoles([FromQuery] TopicRequestGetListByStatusAndRoleQuery query)
    {
        var msg = await _service.GetTopicRequestsForCurrentReviewerByRolesAndStatus<TopicRequestResult>(query);
        return Ok(msg);
    }
    
    //[HttpPost("create-council-requests")]
    //public async Task<IActionResult> CreateCouncilRequests([FromBody] TopicRequestCreateForCouncilsCommand command)
    //{
    //    var result = await _service.CreateCouncilRequestsForIdea(command);
    //    return Ok(result);
    //}
    
    [HttpGet("without-reviewer")]
    public async Task<IActionResult> GetAllUnassignedReviewer([FromQuery] GetQueryableQuery query)
    {
        var msg = await _service.GetAllUnassignedReviewer<TopicRequestResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<TopicRequestResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TopicRequestCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<TopicRequestResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TopicRequestUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<TopicRequestResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] TopicRequestDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] TopicRequestRestoreCommand command)
    {
        var businessResult = await _service.Restore<TopicRequestResult>(command);

        return Ok(businessResult);
    }

    [HttpPut("respond-by-mentor-or-manager")]
    public async Task<IActionResult> RespondByMentorOrManager([FromBody] TopicRequestMentorOrManagerResponseCommand request)
    {
        var businessResult = await _service.RespondByMentorOrManager(request);

        return Ok(businessResult);
    }

    //[HttpPut("council-response")]
    //public async Task<IActionResult> CouncilResponse([FromBody] IdeaVersionRequestLecturerOrCouncilResponseCommand request)
    //{
    //    var businessResult = await _service.CouncilResponse(request);

    //    return Ok(businessResult);
    //}
}
