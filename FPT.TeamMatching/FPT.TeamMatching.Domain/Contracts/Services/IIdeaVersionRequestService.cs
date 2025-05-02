using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaVersionRequestService : IBaseService
    {
        Task<BusinessResult> RespondByMentorOrCouncil(IdeaVersionRequestLecturerOrCouncilResponseCommand command);

        Task CreateVersionRequests(Idea idea, Guid versionId, Guid criteriaFormId);

        Task<BusinessResult> GetAll<TResult>(IdeaVersionRequestGetAllQuery query) where TResult : BaseResult;

        Task<BusinessResult> GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus<TResult>(
            IdeaGetListByStatusAndRoleQuery query)
            where TResult : BaseResult;

        Task<BusinessResult> GetAllUnassignedReviewer<TResult>(GetQueryableQuery query) where TResult : BaseResult;

        Task<BusinessResult> CreateCouncilRequestsForIdea(IdeaVersionRequestCreateForCouncilsCommand command);
    }
}