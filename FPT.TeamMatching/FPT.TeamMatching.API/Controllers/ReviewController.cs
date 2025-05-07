using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Reviews;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_REVIEWS)]
[ApiController]
[Authorize]
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
        var businessResult = await _service.UpdateReview(request);

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
        var businessResult = await _service.ImportExcel(file.file, file.reviewNumber, file.semesterId);
        return Ok(businessResult);
    }

    [HttpPost("get-by-review-number-and-semester-id")]
    public async Task<IActionResult> GetByReviewNumberAndSemesterId([FromBody] ReviewFilterRequest request)
    {
        var businessResult = await _service.GetReviewByReviewNumberAndSemesterIdPaging
                                    (request.Number, request.SemesterId);
        return Ok(businessResult);
    }

    [HttpPut("upload-file-url-review")]
    public async Task<IActionResult> UploadFile(UploadFileUrl request)
    {
        var result = await _service.UpdateFilterReview(request);
        return Ok(result);
    }

    [HttpGet("export-excel-for-reviews")]
    public async Task<IActionResult> ExportExcel()
    {
        // Gọi service để xuất Excel
        var businessResult = await _service.ExportExcelForReviews();

        // Kiểm tra kết quả trả về
        if (businessResult.Status == Const.SUCCESS_CODE && businessResult.Data is byte[] fileBytes)
        {
            // Trả về file Excel cho người dùng
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Lich_3_Buoi_Review.xlsx");
        }

        // Trả về kết quả lỗi nếu không có dữ liệu
        return Ok(businessResult);
    }

    [HttpGet("reviewer")]
    public async Task<IActionResult> GetByReviewerId([FromQuery] Guid reviewerId)
    {
        var result = await _service.GetReviewByReviewerId(reviewerId);
        return Ok(result);
    }

    [HttpPut("update-demo")]
    public async Task<IActionResult> UpdateDemo([FromQuery] Guid reviewId)
    {
        var result = await _service.UpdateReviewDemo(reviewId);
        return Ok(result);
    }
    //[HttpPut("council-assign-reviewers")]
    //public async Task<IActionResult> CouncilAssignReviewers([FromBody] CouncilAssignReviewers request)
    //{
    //    var businessResult = await _service.AssignReviewers(request);

    //    return Ok(businessResult);
    //}
}