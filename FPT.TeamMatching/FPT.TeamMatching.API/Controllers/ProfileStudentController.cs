using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Requests.Queries.ProfileStudents;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_PROFILES)]
[ApiController]
[AllowAnonymous]
public class ProfileStudentController : ControllerBase
{
    private readonly IProfileStudentService _profileStudentService;

    public ProfileStudentController(IProfileStudentService profileStudentService)
    {
        _profileStudentService = profileStudentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile([FromQuery] ProfileStudentGetAllQuery query)
    {
        var msg = await _profileStudentService.GetAllProfiles(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProfileId([FromRoute] Guid id)
    {
        var msg = await _profileStudentService.GetProfileById(id);
        return Ok(msg);
    }

    [HttpGet("/user/{id:guid}")]
    public async Task<IActionResult> GetProfileUser([FromRoute] Guid id)
    {
        var msg = await _profileStudentService.GetProfileByUserId(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] ProfileStudentCreateCommand profileStudent)
    {
        var msg = await _profileStudentService.AddProfile(profileStudent);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileStudentUpdateCommand profileStudent)
    {
        var msg = await _profileStudentService.UpdateProfile(profileStudent);
        return Ok(msg);
    }
}