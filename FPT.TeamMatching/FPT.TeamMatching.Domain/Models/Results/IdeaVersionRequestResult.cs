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
        public Guid? IdeaVersionId { get; set; }

        public Guid? ReviewerId { get; set; }

        public Guid? CriteriaFormId { get; set; }
    
        public IdeaVersionRequestStatus? Status { get; set; }
    
        public string? Role { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public IdeaVersionResult? IdeaVersion { get; set; }

        public UserResult? Reviewer { get; set; }

        public CriteriaFormResult? CriteriaForm { get; set; }

        public ICollection<AnswerCriteriaResult> AnswerCriterias { get; set; } = new List<AnswerCriteriaResult>();
    }
}
