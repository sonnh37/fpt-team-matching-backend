using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaReviews;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaReviews;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_IDEA_REVIEWS)]
[ApiController]
public class IdeaReviewController : ControllerBase
{
    private readonly IIdeaReviewService _service;


    public IdeaReviewController(IIdeaReviewService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] IdeaReviewGetAllQuery query)
    {
        var msg = await _service.GetAll<IdeaReviewResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<IdeaReviewResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IdeaReviewCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<IdeaReviewResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] IdeaReviewUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<IdeaReviewResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] IdeaReviewDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] IdeaReviewRestoreCommand command)
        {
            var businessResult = await _service.Restore<IdeaReviewResult>(command);

            return Ok(businessResult);
        }
    }
