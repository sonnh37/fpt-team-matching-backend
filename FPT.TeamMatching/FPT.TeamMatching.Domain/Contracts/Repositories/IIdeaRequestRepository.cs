using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface IIdeaRequestRepository : IBaseRepository<IdeaRequest>
    {
        Task<(List<IdeaRequest>, int)> GetData(IdeaRequestGetAllQuery query);
        Task<(List<IdeaRequest>, int)> GetDataUnassignedReviewer(GetQueryableQuery query);

        Task<(List<IdeaRequest>, int)> GetIdeaRequestsCurrentByStatus(
            IdeaRequestGetAllCurrentByStatus query, Guid userId);

        Task<(List<IdeaRequest>, int)> GetCurrentIdeaRequestsByStatusAndRoles(
            IdeaRequestGetAllByListStatusForCurrentUser query, Guid userId);

        Task<(List<IdeaRequest>, int)> GetIdeaRequestsByStatusAndRoles(
            IdeaRequestGetAllByListStatusForCurrentUser query);
        
        Task<int> CountApprovedCouncilsForIdea(Guid ideaId);

        Task<int> CountRejectedCouncilsForIdea(Guid ideaId);

        Task<int> CountCouncilsForIdea(Guid ideaId);

    }
}