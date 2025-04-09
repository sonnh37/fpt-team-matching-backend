using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TeamMembers;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_TEAM_MEMBERS)]
[ApiController]
public class TeamMemberController : ControllerBase
{
    private readonly ITeamMemberService _teammemberservice;

    public TeamMemberController(ITeamMemberService _service)
    {
        _teammemberservice = _service;
    }
    // GET: api/<TeamMemberController>

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TeamMemberGetAllQuery query)
    {
        var msg = await _teammemberservice.GetAll<TeamMemberResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _teammemberservice.GetById<TeamMemberResult>(id);
        return Ok(msg);
    }

    [HttpGet("get-by-userid")]
    public async Task<IActionResult> GetByUserId()
    {
        var msg = await _teammemberservice.GetTeamMemberByUserId();
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TeamMemberCreateCommand request)
    {
        var msg = await _teammemberservice.CreateOrUpdate<TeamMemberResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TeamMemberUpdateCommand request)
    {
        var businessResult = await _teammemberservice.CreateOrUpdate<TeamMemberResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] TeamMemberRestoreCommand command)
    {
        var businessResult = await _teammemberservice.Restore<TeamMemberResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] TeamMemberDeleteCommand request)
    {
        var businessResult = await _teammemberservice.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
    
    [HttpDelete("current-user-leave")]
    public async Task<IActionResult> LeaveTeamMemberByCurrentUser()
    {
        var businessResult = await _teammemberservice.LeaveByCurrentUser();

        return Ok(businessResult);
    }

    [HttpPut("update-by-mentor")]
    public async Task<IActionResult> UpdateTeamMemberByMentor([FromBody] List<MentorUpdate> requests)
    {
        var businessResult = await _teammemberservice.UpdateTeamMemberByMentor(requests);

        return Ok(businessResult);
    }

    [HttpPut("update-by-manager")]
    public async Task<IActionResult> UpdateTeamMemberByManager([FromBody] List<ManagerUpdate> requests)
    {
        var businessResult = await _teammemberservice.UpdateTeamMemberByManager(requests);

        return Ok(businessResult);
    }
}