using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface ITopicService: IBaseService
    {
        Task<BusinessResult> GetTopicsForMentor(TopicGetListForMentorQuery query);
        Task<BusinessResult> GetTopicsOfSupervisors<TResult>(TopicGetListOfSupervisorsQuery query) where TResult : BaseResult;
    }
}
