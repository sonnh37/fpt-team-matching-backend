using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface IIdeaVersionRequestRepository : IBaseRepository<IdeaVersionRequest>
    {
        Task<(List<IdeaVersionRequest>, int)> GetData(IdeaVersionRequestGetAllQuery query);
        Task<(List<IdeaVersionRequest>, int)> GetDataUnassignedReviewer(GetQueryableQuery query);

        Task<(List<IdeaVersionRequest>, int)> GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus(
            IdeaVersionRequestGetListByStatusAndRoleQuery query, Guid userId);

        Task<int> CountApprovedCouncilsForIdea(Guid ideaId);

        Task<int> CountRejectedCouncilsForIdea(Guid ideaId);

        Task<int> CountCouncilsForIdea(Guid ideaId);
    }
}