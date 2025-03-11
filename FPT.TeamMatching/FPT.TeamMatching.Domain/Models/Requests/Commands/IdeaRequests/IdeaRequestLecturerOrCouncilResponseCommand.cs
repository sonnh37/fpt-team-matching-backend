using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests
{
    public class IdeaRequestLecturerOrCouncilResponseCommand: UpdateCommand
    {
        public string? Content { get; set; }

        public IdeaRequestStatus? Status { get; set; }
    }
}
