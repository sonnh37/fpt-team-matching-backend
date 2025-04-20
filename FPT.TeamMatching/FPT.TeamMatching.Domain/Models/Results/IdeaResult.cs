using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaResult : BaseResult
    {
        public Guid? OwnerId { get; set; }

        public Guid? MentorId { get; set; }

        public Guid? SubMentorId { get; set; }

        public Guid? SpecialtyId { get; set; }

        public IdeaType? Type { get; set; }

        public IdeaStatus? Status { get; set; }

        public bool IsExistedTeam { get; set; }

        public bool IsEnterpriseTopic { get; set; }

        public UserResult? Owner { get; set; }

        public UserResult? Mentor { get; set; }

        public UserResult? SubMentor { get; set; }

        public SpecialtyResult? Specialty { get; set; }

        public ICollection<IdeaVersionResult> IdeaVersions { get; set; } = new List<IdeaVersionResult>();
    }
}