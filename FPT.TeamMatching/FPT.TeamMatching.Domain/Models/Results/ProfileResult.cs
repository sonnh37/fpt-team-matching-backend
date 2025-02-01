namespace FPT.TeamMatching.Domain.Models.Results;

public class ProfileResult
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

    public string? FileCv { get; set; }

    public string? Department { get; set; }
}