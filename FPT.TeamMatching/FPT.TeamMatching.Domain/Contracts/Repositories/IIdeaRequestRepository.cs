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

        Task<(List<IdeaRequest>, int)> GetDataByStatusAndIdeaId(
            IdeaRequestGetAllByListStatusAndIdeaIdQuery query);
    }
}