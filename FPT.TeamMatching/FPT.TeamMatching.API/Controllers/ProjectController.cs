using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_PROJECTS)]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProjectGetAllQuery query)
    {
        var msg = await _service.GetAll<ProjectResult>(query);
        return Ok(msg);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProjects([FromQuery] ProjectSearchQuery query)
    {
        var result = await _service.SearchProjects(query);
        return Ok(result);
    }

    [HttpGet("me/mentor-projects")]
    public async Task<IActionResult> GetProjectsForMentor([FromQuery] ProjectGetListForMentorQuery query)
    {
        var businessResult = await _service.GetProjectsForMentor(query);
        return Ok(businessResult);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<ProjectResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<ProjectResult>(request);
        return Ok(msg);
    }

    [HttpPost("create-project-with-teammember")]
    public async Task<IActionResult> CreateProjectAndTeamMember([FromBody] TeamCreateCommand request)
    {
        var msg = await _service.CreateProjectAndTeamMember(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProjectUpdateCommand request)
    {
        var businessResult = await _service.CreateOrUpdate<ProjectResult>(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] ProjectDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] ProjectRestoreCommand command)
    {
        var businessResult = await _service.Restore<ProjectResult>(command);

        return Ok(businessResult);
    }

    [HttpGet("get-by-user-id")]
    public async Task<IActionResult> GetByUserIdLogin()
    {
        var businessResult = await _service.GetProjectByUserIdLogin();
        return Ok(businessResult);
    }

    [HttpGet("semester-current/get-by-user-id")]
    public async Task<IActionResult> GetProjectInSemesterCurrentByUserIdLogin()
    {
        var businessResult = await _service.GetProjectInSemesterCurrentByUserIdLogin();
        return Ok(businessResult);
    }

    [HttpGet("get-of-user-login")]
    public async Task<IActionResult> GetProjectOfUserLogin()
    {
        var businessResult = await _service.GetProjectOfUserLogin();
        return Ok(businessResult);
    }

    [HttpGet("export-excel/{defenseStage:int}")]
    public async Task<IActionResult> ExportExcel([FromRoute] int defenseStage)
    {
        // Gọi service để xuất Excel
        var businessResult = await _service.ExportExcelTeamsDefense(defenseStage);

        // Kiểm tra kết quả trả về
        if (businessResult.Status == Const.SUCCESS_CODE && businessResult.Data is byte[] fileBytes)
        {
            // Trả về file Excel cho người dùng
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachNhomDuocRaBaoVe.xlsx");
        }

        // Trả về kết quả lỗi nếu không có dữ liệu
        return Ok(businessResult);
    }

    [HttpPut("update-defense-stage")]
    public async Task<IActionResult> UpdateDefenseStage([FromBody] UpdateDefenseStage command)
    {
        var businessResult = await _service.UpdateDefenseStage(command);

        return Ok(businessResult);
    }

    [HttpGet("get-by-semester-and-stage")]
    public async Task<IActionResult> GetProjectBySemesterAndStage([FromQuery] Guid semester, [FromQuery] int stage)
    {
        var businessResult = await _service.GetProjectBySemesterAndStage(semester, stage);
        return Ok(businessResult);
    }

    [HttpGet("get-project-not-inprogress-yet")]
    public async Task<IActionResult> GetProjectNotInProgressYet()
    {
        var businessResult = await _service.GetProjectNotInProgressYet();
        return Ok(businessResult);
    }

    [HttpGet("get-project-not-canceled")]
    public async Task<IActionResult> GetProjectNotCanceled()
    {
        var businessResult = await _service.GetProjectNotCanceled();
        return Ok(businessResult);
    }

    [HttpPost("create-project-by-manager")]
    public async Task<IActionResult> CreateProjectByManager([FromBody] ProjectCreateByManagerCommand request)
    {
        var result = await _service.ManagerCreateProject(request);
        return Ok(result);
    }

    [HttpGet("submit-block-project-by-student/{projectId:guid}")]
    public async Task<IActionResult> SubmitBlockProjectByStudent(Guid projectId)
    {
        var businessResult = await _service.SubmitBlockProjectByStudent(projectId);
        return Ok(businessResult);
    }

    [HttpPut("block-project-by-manager/{projectId:guid}")]
    public async Task<IActionResult> BlockProjectByManager(Guid projectId)
    {
        var businessResult = await _service.BlockProjectByManager(projectId);
        return Ok(businessResult);
    }
}
