using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class TopicVersionRequest: BaseEntity
    {
        public Guid? TopicVersionId { get; set; }

        public Guid? ReviewerId { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public TopicVersionRequestStatus? Status { get; set; }

        public string? Role { get; set; }

        public string? Feedback { get; set; }

        public virtual TopicVersion? TopicVersion { get; set; }

        public virtual User? Reviewer { get; set; }
    }
}
