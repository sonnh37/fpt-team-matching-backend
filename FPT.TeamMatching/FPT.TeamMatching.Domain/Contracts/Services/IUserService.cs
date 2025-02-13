﻿using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IUserService : IBaseService
{
    Task<BusinessResult> Create(UserCreateCommand command);

    Task<BusinessResult> Update(UserUpdateCommand command);
    // Task<BusinessResult> UpdatePassword(UserPasswordCommand userPasswordCommand);
    //
    // Task<BusinessResult> GetByUsername(string username);
    //
    // BusinessResult SendEmail(string email);
    //
    // BusinessResult ValidateOtp(string email, string otpInput);
    //
    // Task<BusinessResult> RegisterByGoogleAsync(UserCreateByGoogleTokenCommand request);
    //
    // Task<BusinessResult> LoginByGoogleTokenAsync(VerifyGoogleTokenRequest request);
    //
    // Task<BusinessResult> FindAccountRegisteredByGoogle(VerifyGoogleTokenRequest request);
    //
    // Task<BusinessResult> GetByUsernameOrEmail(string key);
    //
    // Task<BusinessResult> GetByRefreshToken(UserGetByRefreshTokenQuery request);
}