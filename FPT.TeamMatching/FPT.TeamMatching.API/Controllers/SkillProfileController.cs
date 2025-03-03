using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfiles;
using FPT.TeamMatching.Domain.Models.Requests.Queries.SkillProfiles;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_SKILLPROFILES)]
[ApiController]
[AllowAnonymous]
public class SkillProfileController : ControllerBase
{
    private readonly ISkillProfileService _skillProfileService;

    public SkillProfileController(ISkillProfileService skillProfileService)
    {
        _skillProfileService = skillProfileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSkillProfiles([FromQuery] SkillProfileGetAllQuery query)
    {
        var msg = await _skillProfileService.GetSkillProfiles(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSkillProfileById([FromRoute] Guid id)
    {
        var msg = await _skillProfileService.GetSkillProfile(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> AddSkillProfile([FromBody] SkillProfileCreateCommand profile)
    {
        var msg = await _skillProfileService.CreateSkillProfile(profile);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSkillProfile([FromBody] SkillProfileUpdateCommand profile)
    {
        var msg = await _skillProfileService.UpdateSkillProfile(profile);
        return Ok(msg);
    }
}