using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ExpirationReviews;
using FPT.TeamMatching.Domain.Models.Requests.Queries.ExpirationReviews;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;
[Route(Const.API_EXPIRATION_REVIEWS)]
[ApiController]
public class ExpirationReviewController : ControllerBase
{
    private readonly IExpirationReviewService _service;

    public ExpirationReviewController(IExpirationReviewService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ExpirationReviewGetAllQuery query)
    {
        var msg = await _service.GetAll<ExpirationReviewResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<ExpirationReviewResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExpirationReviewCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<ExpirationReviewResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ExpirationReviewUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<ExpirationReviewResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] ExpirationReviewRestoreCommand command)
    {
        var businessResult = await _service.Restore<ExpirationReviewResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] ExpirationReviewDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}
