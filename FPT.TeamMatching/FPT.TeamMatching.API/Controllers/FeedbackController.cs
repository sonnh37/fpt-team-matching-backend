using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Feedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Feedbacks;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_FEEDBACKS)]
[ApiController]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _service;


    public FeedbackController(IFeedbackService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FeedbackGetAllQuery query)
    {
        var msg = await _service.GetAll<FeedbackResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<FeedbackResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FeedbackCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<FeedbackResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FeedbackUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<FeedbackResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] FeedbackDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] FeedbackRestoreCommand command)
        {
            var businessResult = await _service.Restore<FeedbackResult>(command);

            return Ok(businessResult);
        }
    }
