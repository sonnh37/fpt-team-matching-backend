using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.AnswerCriterias;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests
{
    public class IdeaVersionRequestLecturerOrCouncilResponseCommand : UpdateCommand
    {
        public IdeaVersionRequestStatus? Status { get; set; }

        public List<AnswerCriteriaForLecturerRespond>? AnswerCriteriaList { get; set; }
    }
}
