using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistories;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaHistoryService: IBaseService
    {
        Task<BusinessResult> StudentUpdateIdea(StudentUpdateIdeaCommand request);
        Task<BusinessResult> LecturerUpdate(LecturerUpdateCommand request);
        Task<BusinessResult> GetAllIdeaHistoryByIdeaId(Guid ideaId);
    }
}
