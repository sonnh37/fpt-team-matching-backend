using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;

public class ProfileStudentCreateCommand : CreateCommand
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

    public IFormFile? FileCv { get; set; }
}