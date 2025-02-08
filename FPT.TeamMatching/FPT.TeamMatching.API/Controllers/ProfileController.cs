using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Profile;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Profile;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_PROFILES)]
[ApiController]
[AllowAnonymous]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile([FromQuery] ProfileGetAllQuery query)
    {
        var msg = await _profileService.GetAllProfiles(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProfileId([FromRoute] Guid id)
    {
        var msg = await _profileService.GetProfileById(id);
        return Ok(msg);
    }

    [HttpGet("/user/{id:guid}")]
    public async Task<IActionResult> GetProfileUser([FromRoute] Guid id)
    {
        var msg = await _profileService.GetProfileByUserId(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] ProfileCreateCommand profile)
    {
        var msg = await _profileService.AddProfile(profile);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateCommand profile)
    {
        var msg = await _profileService.UpdateProfile(profile);
        return Ok(msg);
    }
}