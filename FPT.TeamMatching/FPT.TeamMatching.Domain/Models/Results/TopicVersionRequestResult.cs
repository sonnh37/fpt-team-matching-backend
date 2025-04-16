using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class TopicVersionRequestResult: BaseResult
    {
        public Guid? TopicVersionId { get; set; }

        public Guid? ReviewerId { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public TopicVersionRequestStatus? Status { get; set; }

        public string? Role { get; set; }

        public virtual TopicVersionResult? TopicVersion { get; set; }

        public virtual UserResult? Reviewer { get; set; }
    }
}
