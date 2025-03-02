using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;

public class ProfileStudentUpdateCommand
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string? Bio { get; set; }

    public string? Code { get; set; }

    public bool IsQualifiedForAcademicProject { get; set; }

    public string? Major { get; set; }

    public string? Achievement { get; set; }

    public string? Semester { get; set; }

    public string? ExperienceProject { get; set; }

    public string? Interest { get; set; }

    public IFormFile? FileCv { get; set; }

    public string? Department { get; set; }
}