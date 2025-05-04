using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorTopicRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorTopicRequests;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IMentorTopicRequestService: IBaseService
    {
        Task<BusinessResult> StudentRequestTopic(StudentRequest request);

        Task<BusinessResult> GetUserMentorTopicRequests(MentorTopicRequestGetAllQuery query);
        Task<BusinessResult> GetMentorMentorTopicRequests(MentorTopicRequestGetAllQuery query);
        Task<BusinessResult> UpdateMentorTopicRequestStatus(MentorTopicRequestUpdateCommand request);
    }
}
