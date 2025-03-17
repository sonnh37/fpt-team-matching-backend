using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Reviews;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_REVIEWS)]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewController(IReviewService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ReviewGetAllQuery query)
    {
        var msg = await _service.GetAll<ReviewResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<ReviewResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<ReviewResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ReviewUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<ReviewResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] ReviewDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpGet("get-by-projectId/{projectId:guid}")]
    public async Task<IActionResult> GetByProjectId([FromRoute] Guid projectId)
    {
        var msg = await _service.GetByProjectId(projectId);
        return Ok(msg);
    }

    [HttpPut("student-submit-review")]
    public async Task<IActionResult> StudentSubmitReview([FromBody] SubmitReviewCommand request)
    {
        var businessResult = await _service.StudentSubmitReview(request);

        return Ok(businessResult);
    }

    [HttpPost("import-file-excel-review")]
    public async Task<IActionResult> ImportExcel([FromForm] FileImport file)
    {
        var businessResult = await _service.ImportExcel(file.file, file.reviewNumber);
        return Ok(businessResult);
    }
    //[HttpPut("council-assign-reviewers")]
    //public async Task<IActionResult> CouncilAssignReviewers([FromBody] CouncilAssignReviewers request)
    //{
    //    var businessResult = await _service.AssignReviewers(request);

    //    return Ok(businessResult);
    //}
}