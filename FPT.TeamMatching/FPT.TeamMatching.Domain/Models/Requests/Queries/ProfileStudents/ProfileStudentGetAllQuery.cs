using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.ProfileStudents;

public class ProfileStudentGetAllQuery : GetQueryableQuery
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
}