using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IMentorFeedbackService: IBaseService
    {
        Task<BusinessResult> UpdateMentorFeedbackAfterReview3(MentorFeedbackUpdateCommand command);
    }
}
