using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorIdeaRequest;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface IMentorIdeaRequestRepository: IBaseRepository<MentorIdeaRequest>
    {
        Task<(List<MentorIdeaRequest>, int)> GetUserMentorIdeaRequests(MentorIdeaRequestGetAllQuery query,
            Guid userId);
        Task<(List<MentorIdeaRequest>, int)> GetMentorMentorIdeaRequests(MentorIdeaRequestGetAllQuery query,
            Guid userId);
        Task<List<MentorIdeaRequest>?> GetByIdeaId(Guid ideaId);
    }
}
