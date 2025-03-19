
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.MentorIdeaRequests
{
    public class MentorIdeaRequestCreateCommand: CreateCommand
    {
        public Guid? ProjectId { get; set; }

        public Guid? IdeaId { get; set; }

        public MentorIdeaRequestStatus? Status { get; set; }
    }
}
