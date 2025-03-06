using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaRequestService: IBaseService
    {
        Task<BusinessResult> LecturerResponse(IdeaRequestLecturerOrCouncilResponseCommand command);
        Task<BusinessResult> CouncilResponse(IdeaRequestLecturerOrCouncilResponseCommand command);
        Task<BusinessResult> GetAllByListStatusAndIdea(IdeaRequestGetAllByStatusQuery query);
    }
}
