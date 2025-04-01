using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class MentorFeedback: BaseEntity
    {
        public Guid? ProjectId { get; set; }

        public string? ThesisContent { get; set; }

        public string? ThesisForm { get; set; }

        public string? AchievementLevel { get; set; }

        public string? Limitation { get; set; }

        public virtual Project? Project { get; set; }
    }
}
