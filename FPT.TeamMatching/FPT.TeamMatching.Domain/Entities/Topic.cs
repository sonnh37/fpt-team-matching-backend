using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class Topic: BaseEntity
    {
        public Guid? OwnerId { get; set; }

        public Guid? MentorId { get; set; }

        public Guid? SubMentorId { get; set; }

        public Guid? SpecialtyId { get; set; }

        public Guid? StageTopicId { get; set; }

        public string? TopicCode { get; set; }

        public TopicType? Type { get; set; }

        public TopicStatus? Status { get; set; }

        public bool IsExistedTeam { get; set; }

        public string? VietNameseName { get; set; }

        public string? EnglishName { get; set; }

        public string? Description { get; set; }

        public string? Abbreviation { get; set; }

        public bool IsEnterpriseTopic { get; set; }

        public string? EnterpriseName { get; set; }

        public string? FileUrl { get; set; }

        public virtual Project? Project { get; set; }

        public virtual User? Owner { get; set; }

        public virtual User? Mentor { get; set; }

        public virtual User? SubMentor { get; set; }

        public virtual Specialty? Specialty { get; set; }

        public virtual StageTopic? StageTopic { get; set; }

        public virtual ICollection<TopicRequest> TopicRequests { get; set; } = new List<TopicRequest>();

        public virtual ICollection<TopicVersion> TopicVersions { get; set; } = new List<TopicVersion>();

        public virtual ICollection<MentorTopicRequest> MentorTopicRequests { get; set; } = new List<MentorTopicRequest>();
    }
}
