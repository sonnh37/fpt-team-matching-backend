using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersionRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface ITopicVersionRequestService: IBaseService
    {
        Task<BusinessResult> RespondByLecturerOrManager(RespondByMentorOrManager request);
    }
}
