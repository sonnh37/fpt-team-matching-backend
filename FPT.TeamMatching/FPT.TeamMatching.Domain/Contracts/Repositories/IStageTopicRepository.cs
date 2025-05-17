using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface IStageTopicRepository: IBaseRepository<StageTopic>
    {
        Task<StageTopic?> GetByStageNumberAndSemester(int number, Guid semesterId);

        Task<StageTopic?> GetCurrentStageTopic();

        Task<StageTopic?> GetCurrentStageTopicBySemesterId (Guid semesterId);
    }
}
