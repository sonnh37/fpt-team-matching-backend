using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface ITopicRequestRepository : IBaseRepository<TopicRequest>
    {
        Task<(List<TopicRequest>, int)> GetData(TopicRequestGetAllQuery query);

        Task<(List<TopicRequest>, int)> GetDataExceptPending(TopicRequestGetAllQuery query);

        Task<(List<TopicRequest>, int)> GetDataUnassignedReviewer(GetQueryableQuery query);

        Task<int> CountStatusCouncilsForIdea(Guid ideaId, TopicRequestStatus status);

        Task<(List<TopicRequest>, int)> GetTopicVersionRequestsForCurrentReviewerByRolesAndStatus(
            TopicRequestGetListByStatusAndRoleQuery query, Guid userId);

        Task<int> CountCouncilsForIdea(Guid ideaId);

        Task<List<TopicRequest>?> GetRoleMentorNotApproveInSemester(Guid semesterId);

        Task<List<TopicRequest>> GetByTopicIdAndRole(Guid topicId, string role);

        Task<List<TopicRequest>> GetByTopicIdAndRoleAndStatus(Guid topicId, string role, TopicRequestStatus status);
    }
}