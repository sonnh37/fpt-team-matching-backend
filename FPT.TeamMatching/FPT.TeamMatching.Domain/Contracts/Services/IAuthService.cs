using System.Security.Cryptography;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IAuthService
{
    Task<BusinessResult> Login(AuthQuery authQuery);

    Task<RSA> GetRSAKeyFromTokenAsync(string token, string kid);

    Task<BusinessResult> GetUserByCookie();

    Task<BusinessResult> RefreshToken(UserRefreshTokenCommand request);

    Task<BusinessResult> Logout(UserLogoutCommand userLogoutCommand);

    Task<BusinessResult> LoginByGoogleTokenAsync(AuthByGoogleTokenQuery request);
}