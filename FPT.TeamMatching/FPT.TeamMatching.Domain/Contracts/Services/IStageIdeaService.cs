using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IStageIdeaService: IBaseService
    {
        Task<BusinessResult> GetByStageNumber<TResult>(int number) where TResult : BaseResult;

        Task<BusinessResult> GetLatestStageIdea<TResult>() where TResult : BaseResult;
    }
}
