using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ProfileStudentResult : BaseResult
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
    
    public SpecialtyResult? Specialty { get; set; }

    public UserResult? User { get; set; }
    
    public virtual SemesterResult? Semester { get; set; }

    public ICollection<SkillProfileResult> SkillProfiles { get; set; } = new List<SkillProfileResult>();
}