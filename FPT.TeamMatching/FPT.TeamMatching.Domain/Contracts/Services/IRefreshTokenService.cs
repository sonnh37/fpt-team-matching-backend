using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IRefreshTokenService : IBaseService
{
    Task<BusinessResult> CreateOrUpdate<TResult>(CreateOrUpdateCommand createOrUpdateCommand)
        where TResult : BaseResult;

    BusinessResult ValidateRefreshTokenIpMatch();
}