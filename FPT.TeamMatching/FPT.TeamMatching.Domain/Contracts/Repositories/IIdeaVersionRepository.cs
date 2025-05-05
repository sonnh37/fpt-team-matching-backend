using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface IIdeaVersionRepository: IBaseRepository<IdeaVersion>
    {
        Task<List<IdeaVersion>> GetIdeaVersionsByIdeaId(Guid ideaId);

        Task<IdeaVersion?> GetLastIdeaVersionByTopicId(Guid topicId);
    }
}
