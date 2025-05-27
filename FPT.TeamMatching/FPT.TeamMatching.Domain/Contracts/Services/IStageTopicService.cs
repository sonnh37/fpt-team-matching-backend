using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.StageTopics;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IStageTopicService: IBaseService
    {
        Task<BusinessResult> GetByStageNumber<TResult>(int number) where TResult : BaseResult;

        Task<BusinessResult> GetCurrentStageTopic<TResult>() where TResult : BaseResult;

        Task<BusinessResult> ShowResult(Guid stageTopicId);

        Task<BusinessResult> Create(StageTopicCreateCommand command);
        Task<BusinessResult> Update(StageTopicUpdateCommand command);
    }
}
