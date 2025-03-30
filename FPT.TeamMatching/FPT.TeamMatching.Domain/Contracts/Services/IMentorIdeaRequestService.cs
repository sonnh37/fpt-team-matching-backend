using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorIdeaRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorIdeaRequest;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IMentorIdeaRequestService: IBaseService
    {
        Task<BusinessResult> StudentRequestIdea(StudentRequest request);

        Task<BusinessResult> GetUserMentorIdeaRequests(MentorIdeaRequestGetAllQuery query);
        Task<BusinessResult> GetMentorMentorIdeaRequests(MentorIdeaRequestGetAllQuery query);
        Task<BusinessResult> UpdateMentorIdeaRequestStatus(MentorIdeaRequestUpdateCommand request);
    }
}
