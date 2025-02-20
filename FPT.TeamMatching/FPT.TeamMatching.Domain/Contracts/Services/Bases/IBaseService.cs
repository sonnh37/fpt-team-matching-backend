using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services.Bases;

// public interface IBaseService
// {
//     
// }
public interface IBaseService
{
    Task<BusinessResult> GetAll<TResult>() where TResult : BaseResult;

    // BusinessResult GetUserByCookie();

    Task<BusinessResult> GetAll<TResult>(GetQueryableQuery query) where TResult : BaseResult;

    Task<BusinessResult> GetById<TResult>(Guid id) where TResult : BaseResult;

    Task<BusinessResult> DeleteById(Guid id, bool isPermanent = false);

    Task<BusinessResult> CreateOrUpdate<TResult>(CreateOrUpdateCommand createOrUpdateCommand)
        where TResult : BaseResult;

    Task<BusinessResult> Restore<TResult>(UpdateCommand updateCommand) where TResult : BaseResult;
}