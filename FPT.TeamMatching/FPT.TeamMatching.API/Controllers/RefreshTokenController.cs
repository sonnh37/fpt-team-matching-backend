using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Requests.Queries.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_REFRESH_TOKENS)]
[ApiController]
[AllowAnonymous]
public class RefreshTokenController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenController(IRefreshTokenService __refreshTokenService)
    {
        _refreshTokenService = __refreshTokenService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] RefreshTokenGetAllQuery query)
    {
        var msg = await _refreshTokenService.GetAll<RefreshTokenResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _refreshTokenService.GetById<RefreshTokenResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RefreshTokenCreateCommand request)
    {
        var msg = await _refreshTokenService.CreateOrUpdate<RefreshTokenResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RefreshTokenUpdateCommand request)
    {
        var businessResult = await _refreshTokenService.CreateOrUpdate<RefreshTokenResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] RefreshTokenRestoreCommand command)
    {
        var businessResult = await _refreshTokenService.Restore<RefreshTokenResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] RefreshTokenDeleteCommand request)
    {
        var businessResult = await _refreshTokenService.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}