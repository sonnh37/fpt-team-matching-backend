
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class MentorFeedbackResult: BaseResult
    {
        public Guid? ProjectId { get; set; }

        public string? ThesisContent { get; set; }

        public string? ThesisForm { get; set; }

        public string? AchievementLevel { get; set; }

        public string? Limitation { get; set; }

        public virtual ProjectResult? Project { get; set; }
    }
}
