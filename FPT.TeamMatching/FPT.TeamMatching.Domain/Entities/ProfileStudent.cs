using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class ProfileStudent : BaseEntity
{
    public Guid? UserId { get; set; }
    
    public Guid? SpecialtyId { get; set; }
    
    public Guid? SemesterId { get; set; }

    public string? Bio { get; set; }

    public string? Code { get; set; }

    public bool IsQualifiedForAcademicProject { get; set; }

    public string? Achievement { get; set; }
    
    public string? ExperienceProject { get; set; }

    public string? Interest { get; set; }

    public string? FileCv { get; set; }
    
    public virtual Specialty? Specialty { get; set; }

    public virtual User? User { get; set; }
    
    public virtual Semester? Semester { get; set; }

    public virtual ICollection<SkillProfile> SkillProfiles { get; set; } = new List<SkillProfile>();
}