using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.MentorFeedbacks
{
    public class MentorFeedbackUpdateCommand: UpdateCommand
    {
        public Guid? ProjectId { get; set; }

        public string? ThesisContent { get; set; }

        public string? ThesisForm { get; set; }

        public string? AchievementLevel { get; set; }

        public string? Limitation { get; set; }
    }
}
