using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface ITopicRequestService : IBaseService
    {
        Task<BusinessResult> RespondByMentorOrManager(TopicRequestMentorOrManagerResponseCommand command);

        Task CreateVersionRequests(Topic topic, Guid versionId, Guid criteriaFormId);
        Task CreateVersionRequesForFirstCreateTopic(Topic topic, Guid versionId, Guid criteriaFormId);

        Task<BusinessResult> GetAll<TResult>(TopicRequestGetAllQuery query) where TResult : BaseResult;

        Task<BusinessResult> GetAllExceptPending<TResult>(TopicRequestGetAllQuery query) where TResult : BaseResult;

        Task<BusinessResult> GetTopicRequestsForCurrentReviewerByRolesAndStatus<TResult>(
            TopicRequestGetListByStatusAndRoleQuery query)
            where TResult : BaseResult;

        Task<BusinessResult> GetAllUnassignedReviewer<TResult>(GetQueryableQuery query) where TResult : BaseResult;

        Task<BusinessResult> CreateCouncilRequestsForTopic(TopicRequestCreateForCouncilsCommand command);
    }
}