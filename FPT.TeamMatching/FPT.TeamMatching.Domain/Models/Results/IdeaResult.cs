using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaResult: BaseResult
    {
        public Guid? UserId { get; set; }

        public Guid? SemesterId { get; set; }

        public Guid? SubMentorId { get; set; }

        public IdeaType? Type { get; set; }
    
        public Guid? SpecialtyId { get; set; }

        public string? IdeaCode { get; set; }
    
        public string? Description { get; set; }
    
        public string? Abbreviations { get; set; }

        public string? VietNamName { get; set; }

        public string? EnglishName { get; set; }

        public string? File { get; set; }

        public IdeaStatus? Status { get; set; }

        public bool IsExistedTeam { get; set; }
        
        public bool IsEnterpriseTopic { get; set; }
        
        public string? EnterpriseName { get; set; }

        public int? MaxTeamSize { get; set; }

        public virtual UserResult? User { get; set; }

        public virtual UserResult? SubMentor { get; set; }
    
        public virtual SpecialtyResult? Specialty { get; set; }

        public virtual ProjectResult? Project { get; set; }

        public virtual SemesterResult? Semester { get; set; }

        public virtual ICollection<IdeaReviewResult> IdeaReviews { get; set; } = new List<IdeaReviewResult>();
    }
}
