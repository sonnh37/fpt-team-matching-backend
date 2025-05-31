using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_USERS)]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService __userService)
    {
        _userService = __userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserGetAllQuery query)
    {
        var msg = await _userService.GetAll<UserResult>(query);
        return Ok(msg);
    }

    [HttpGet("current-semester")]
    public async Task<IActionResult> GetUsersInSemester([FromQuery] UserGetAllInSemesterQuery query)
    {
        var msg = await _userService.GetUsersInSemester(query);
        return Ok(msg);
    }
    
    [HttpGet("student/no-team")]
    public async Task<IActionResult> GetStudentsNoTeam([FromQuery] UserGetAllQuery query)
    {
        var msg = await _userService.GetStudentsNoTeam(query);
        return Ok(msg);
    }

    //[HttpGet("council/pending-ideas")]
    //public async Task<IActionResult> GetAllByCouncilWithIdeaVersionRequestPending([FromQuery] UserGetAllQuery query)
    //{
    //    var msg = await _userService.GetAllByCouncilWithIdeaVersionRequestPending(query);
    //    return Ok(msg);
    //}


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _userService.GetById<UserResult>(id);
        return Ok(msg);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail([FromRoute] string email)
    {
        var msg = await _userService.GetByEmail<UserResult>(email);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateCommand request)
    {
        var msg = await _userService.CreateOrUpdate<UserResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserUpdateCommand request)
    {
        var businessResult = await _userService.CreateOrUpdate<UserResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("update-cache")]
    public async Task<IActionResult> UpdateUserCache([FromBody] UserUpdateCacheCommand request)
    {
        var businessResult = await _userService.UpdateUserCacheAsync(request);

        return Ok(businessResult);
    }


    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] UserRestoreCommand command)
    {
        var businessResult = await _userService.Restore<UserResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] UserDeleteCommand request)
    {
        var businessResult = await _userService.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UserPasswordCommand userUpdateCommand)
    {
        var businessResult = await _userService.UpdatePassword(userUpdateCommand);

        return Ok(businessResult);
    }

    [HttpGet("get-student-do-not-have-team")]
    public async Task<IActionResult> GetStudentDoNotHaveTeam([FromQuery] Guid semesterId)
    {
        var msg = await _userService.GetStudentDoNotHaveTeam(semesterId);
        return Ok(msg);
    }

    [HttpGet("role/reviewer")]
    public async Task<IActionResult> GetRoleReviewer()
    {
        var msg = await _userService.GetAllReviewer();
        return Ok(msg);
    }

    [HttpPost("import/students/many")]
    public async Task<IActionResult> ImportStudents([FromForm] ImportUserModel file)
    {
        var msg = await _userService.ImportStudents(file.file, file.semesterId);
        return Ok(msg);
    }

    [HttpPost("import/students/one")]
    public async Task<IActionResult> ImportStudent([FromBody] CreateByManagerCommand command)
    {
        var msg = await _userService.ImportStudent(command);
        return Ok(msg);
    }

    [HttpPost("import/lecturers/many")]
    public async Task<IActionResult> ImportLecturers([FromForm] ImportUserModel file)
    {
        var msg = await _userService.ImportLecturers(file.file);
        return Ok(msg);
    }

    [HttpPost("import/lecturers/one")]
    public async Task<IActionResult> ImportLecturer([FromBody] CreateByManagerCommand command)
    {
        var msg = await _userService.ImportLecturer(command);
        return Ok(msg);
    }

    [HttpPut("import/students/update-existed")]
    public async Task<IActionResult> UpdateStudentExistedRange([FromBody] UserResult[] userResults, [FromQuery] Guid semesterId)
    {
        var msg = await _userService.UpdateStudentExistedRange(userResults, semesterId);
        return Ok(msg);
    }
    
    [HttpPut("import/lecturers/update-existed")]
    public async Task<IActionResult> UpdateLecturerExistedRange([FromBody] UserResult[] userResults, [FromQuery] Guid semesterId)
    {
        var msg = await _userService.UpdateLecturerExistedRange(userResults, semesterId);
        return Ok(msg);
    }

    [HttpGet("get-suggestions-emails")]
    public async Task<IActionResult> GetTeamMembers([FromQuery] string email)
    {
        var msg = await _userService.GetSuggestionByEmail(email);
        return Ok(msg);
    }

    [HttpGet("check-mentor-and-submentor-slot-availability")]
    public async Task<IActionResult> CheckMentorAndSubMentorSlotAvailability([FromQuery] UserCheckMentorAndSubMentorQuery query)
    {
        var hasProjects = await _userService.CheckMentorAndSubMentorSlotAvailability(query.MentorId, query.SubMentorId);
        return Ok(hasProjects);
    }
}