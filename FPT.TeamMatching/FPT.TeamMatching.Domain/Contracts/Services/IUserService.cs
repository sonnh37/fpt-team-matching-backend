﻿using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IUserService : IBaseService
{
    Task<BusinessResult> Create(UserCreateCommand command);

    Task<BusinessResult> UpdatePassword(UserPasswordCommand userPasswordCommand);

    Task<BusinessResult> UpdateUserCacheAsync(UserUpdateCacheCommand newCacheJson);

    Task<BusinessResult> GetAllByCouncilWithIdeaRequestPending(UserGetAllQuery query);
    
    Task<BusinessResult> GetByEmail<TResult>(string email) where TResult : BaseResult;


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

    Task<BusinessResult> GetStudentDoNotHaveTeam();
    Task<BusinessResult> GetAllReviewer();
}