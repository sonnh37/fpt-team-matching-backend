﻿using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_IDEA_REQUESTS)]
[ApiController]
public class IdeaRequestController : ControllerBase
{
    private readonly IIdeaRequestService _service;


    public IdeaRequestController(IIdeaRequestService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] IdeaRequestGetAllQuery query)
    {
        var msg = await _service.GetAll<IdeaRequestResult>(query);
        return Ok(msg);
    }
    
    [HttpGet("by-list-status-and-idea-id")]
    public async Task<IActionResult> GetAllByStatusAndIdeaId([FromQuery] IdeaRequestGetAllByListStatusAndIdeaIdQuery query)
    {
        var msg = await _service.GetAllByStatusAndIdeaId<IdeaRequestResult>(query);
        return Ok(msg);
    }
    
    [HttpGet("me/by-list-status")]
    public async Task<IActionResult> GetAllByStatusForCurrentUser([FromQuery] IdeaRequestGetAllByListStatusForCurrentUser query)
    {
        var msg = await _service.GetAllByStatusForCurrentUser<IdeaRequestResult>(query);
        return Ok(msg);
    }
    
    [HttpGet("without-reviewer")]
    public async Task<IActionResult> GetAllUnassignedReviewer([FromQuery] GetQueryableQuery query)
    {
        var msg = await _service.GetAllUnassignedReviewer<IdeaRequestResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<IdeaRequestResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IdeaRequestCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<IdeaRequestResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] IdeaRequestUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<IdeaRequestResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] IdeaRequestDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] IdeaRequestRestoreCommand command)
    {
        var businessResult = await _service.Restore<IdeaRequestResult>(command);

        return Ok(businessResult);
    }

    [HttpPut("lecturer-response")]
    public async Task<IActionResult> LecturerResponse([FromBody] IdeaRequestLecturerOrCouncilResponseCommand request)
    {
        var businessResult = await _service.LecturerResponse(request);

        return Ok(businessResult);
    }

    [HttpPut("council-response")]
    public async Task<IActionResult> CouncilResponse([FromBody] IdeaRequestLecturerOrCouncilResponseCommand request)
    {
        var businessResult = await _service.CouncilResponse(request);

        return Ok(businessResult);
    }
}
