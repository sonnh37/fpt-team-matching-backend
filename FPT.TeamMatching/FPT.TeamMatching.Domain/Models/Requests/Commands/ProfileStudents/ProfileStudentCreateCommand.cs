using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;

public class ProfileStudentCreateCommand : CreateCommand
{
    public Guid? UserId { get; set; }
    
    public Guid? SpecialtyId { get; set; }
    
    public Guid? SemesterId { get; set; }

    public string? Bio { get; set; }

    public string? Achievement { get; set; }

    public string? ExperienceProject { get; set; }

    public string? Interest { get; set; }

    public string? FileCv { get; set; }
    
    // add more
    public string? Json {get; set;}
    public string? FullSkill {get; set;}
}