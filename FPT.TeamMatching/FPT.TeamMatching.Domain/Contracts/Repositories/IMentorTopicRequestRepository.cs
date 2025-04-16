using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorTopicRequests;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface IMentorTopicRequestRepository: IBaseRepository<MentorTopicRequest>
    {
        Task<(List<MentorTopicRequest>, int)> GetUserMentorTopicRequests(MentorTopicRequestGetAllQuery query,
            Guid userId);
        Task<(List<MentorTopicRequest>, int)> GetMentorMentorTopicRequests(MentorTopicRequestGetAllQuery query,
            Guid userId);
        Task<List<MentorTopicRequest>?> GetByIdeaId(Guid ideaId);
    }
}
