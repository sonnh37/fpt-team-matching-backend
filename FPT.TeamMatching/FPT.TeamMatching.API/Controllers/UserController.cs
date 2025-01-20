using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_USERS)]
[ApiController]
[AllowAnonymous]
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _userService.GetById<UserResult>(id);
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
}