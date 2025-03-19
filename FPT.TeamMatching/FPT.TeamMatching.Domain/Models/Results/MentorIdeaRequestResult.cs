using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class MentorIdeaRequestResult: BaseResult
    {
        public Guid? ProjectId { get; set; }

        public Guid? IdeaId { get; set; }

        public MentorIdeaRequestStatus? Status { get; set; }
    }
}
