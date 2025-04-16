using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaVersionRequestResult: BaseResult
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }
        
        public IdeaVersionRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
        
        public string? Role { get; set; }

        public IdeaResult? Idea { get; set; }

        public UserResult? Reviewer { get; set; }
    }
}
