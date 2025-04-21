using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using Microsoft.AspNetCore.Http;
using BaseResult = FPT.TeamMatching.Domain.Models.Results.Bases.BaseResult;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IUserService : IBaseService
{
    Task<BusinessResult> Create(UserCreateCommand command);

    Task<BusinessResult> UpdatePassword(UserPasswordCommand userPasswordCommand);

    Task<BusinessResult> UpdateUserCacheAsync(UserUpdateCacheCommand newCacheJson);

    Task<BusinessResult> GetAllByCouncilWithIdeaVersionRequestPending(UserGetAllQuery query);
    
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
    Task<BusinessResult> ImportStudents(IFormFile file);
    Task<BusinessResult> ImportStudent(CreateByManagerCommand command);
    Task<BusinessResult> ImportLecturers(IFormFile file);
    Task<BusinessResult> ImportLecturer(CreateByManagerCommand command);
    Task<BusinessResult> UpdateStudentExistedRange(UserResult[] users);
    Task<BusinessResult> GetSuggestionByEmail(string email);
}