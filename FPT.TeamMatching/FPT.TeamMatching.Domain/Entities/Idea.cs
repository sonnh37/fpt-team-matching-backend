using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Idea : BaseEntity
{
    public Guid? UserId { get; set; }
    
    public Guid? SemesterId { get; set; }
    
    public Guid? SubMentorId { get; set; }

    public IdeaType? Type { get; set; }
    
    public string? Title { get; set; }
    
    public string? IdeaCode { get; set; }
    
    public string? VietNamName { get; set; }
    
    public string? EnglishName { get; set; }
    
    public string? Major { get; set; }
    
    public string? File { get; set; }
    
    public string? Description { get; set; }

    public ProjectStatus? Status { get; set; }

    public bool? IsExistedTeam { get; set; }
    
    public int? MaxTeamSize { get; set; }

    public virtual User? User { get; set; }
    
    public virtual User? SubMentor { get; set; }
    
    public virtual Semester? Semester { get; set; }
    
    public virtual ICollection<IdeaReview> IdeaReviews { get; set; } = new List<IdeaReview>();
}