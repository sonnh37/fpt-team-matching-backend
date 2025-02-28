using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaService: IBaseService
    {
        Task<BusinessResult> GetAll<TResult>(IdeaGetAllQuery query) where TResult : BaseResult;
        Task<BusinessResult> StudentCreatePending(IdeaStudentCreatePendingCommand idea);
        Task<BusinessResult> LecturerCreatePending(IdeaLecturerCreatePendingCommand idea);
        Task<BusinessResult> GetIdeasByUserId();
    }
}
