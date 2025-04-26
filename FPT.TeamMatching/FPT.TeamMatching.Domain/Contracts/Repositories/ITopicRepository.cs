using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface ITopicRepository: IBaseRepository<Topic>
    {
        int NumberOfTopicBySemesterId(Guid semesterId);
        bool IsExistedTopicCode(string topicCode);
        Task<List<Topic>> GetAllTopicsByTopicCode(string[] topicCodes);
        Task<(List<Topic>, int)> GetTopicsForMentor(TopicGetListForMentorQuery query, Guid userId);
        Task<List<Topic>> GetTopicByIdeaVersionId(List<Guid?> ideaVersionIds);
    }
}
