﻿using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_AUTH)]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var businessResult = await _authService.GetUserByCookie();

        return Ok(businessResult);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthQuery request)
    {
        var businessResult = await _authService.Login(request);

        return Ok(businessResult);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var userLogoutCommand = new UserLogoutCommand
        {
            RefreshToken = refreshToken
        };

        var businessResult = await _authService.Logout(userLogoutCommand);

        return Ok(businessResult);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var request = new UserRefreshTokenCommand
        {
            RefreshToken = refreshToken
        };

        var businessResult = await _authService.RefreshToken(request);

        return Ok(businessResult);
    }

    [AllowAnonymous]
    // POST api/<AuthController>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateCommand request)
    {
        var businessResult = await _userService.Create(request);
        return Ok(businessResult);
    }

    // [AllowAnonymous]
    // [HttpPost("verify-otp")]
    // public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPQuery request)
    // {
    //     var businessResult = await _mediator.Send(request);
    //     return Ok(businessResult);
    // }
    //
    [AllowAnonymous]
    [HttpPost("login-by-google")]
    public async Task<IActionResult> LoginByGoogle([FromBody] AuthByGoogleTokenQuery request)
    {
        var businessResult = await _authService.LoginByGoogleTokenAsync(request);
        return Ok(businessResult);
    }
    //
    // [AllowAnonymous]
    // [HttpPost("register-by-google")]
    // public async Task<IActionResult> RegisterByGoogle([FromBody] UserCreateByGoogleTokenCommand request)
    // {
    //     var businessResult = await _mediator.Send(request);
    //     return Ok(businessResult);
    // }
}