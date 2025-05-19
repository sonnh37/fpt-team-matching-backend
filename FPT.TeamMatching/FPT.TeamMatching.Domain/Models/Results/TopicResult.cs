using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class TopicResult : BaseResult
    {
        public Guid? OwnerId { get; set; }

        public Guid? MentorId { get; set; }

        public Guid? SubMentorId { get; set; }

        public Guid? SpecialtyId { get; set; }

        public Guid? StageTopicId { get; set; }

        public Guid? SemesterId { get; set; }

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

        public virtual ProjectResult? Project { get; set; }

        public virtual UserResult? Owner { get; set; }

        public virtual UserResult? Mentor { get; set; }

        public virtual UserResult? SubMentor { get; set; }

        public virtual SpecialtyResult? Specialty { get; set; }

        public virtual StageTopicResult? StageTopic { get; set; }

        public virtual SemesterResult? Semester { get; set; }

        public virtual ICollection<TopicRequestResult> TopicRequests { get; set; } = new List<TopicRequestResult>();

        public virtual ICollection<TopicVersionResult> TopicVersions { get; set; } = new List<TopicVersionResult>();

        public virtual ICollection<MentorTopicRequestResult> MentorTopicRequests { get; set; } = new List<MentorTopicRequestResult>();

    }
}