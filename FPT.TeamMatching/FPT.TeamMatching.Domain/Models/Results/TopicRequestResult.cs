using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class TopicRequestResult: BaseResult
    {
        public Guid? TopicId { get; set; }

        public Guid? ReviewerId { get; set; }

        public Guid? CriteriaFormId { get; set; }

        public TopicRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public string? Role { get; set; }

        public virtual TopicResult? Topic { get; set; }

        public virtual UserResult? Reviewer { get; set; }

        public virtual CriteriaFormResult? CriteriaForm { get; set; }

        public virtual ICollection<AnswerCriteriaResult> AnswerCriterias { get; set; } = new List<AnswerCriteriaResult>();
    }
}
