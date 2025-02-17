using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class UserResult : BaseResult
{
    public Gender? Gender { get; set; }

    public string? Cache { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<BlogResult> Blogs { get; set; } = new List<BlogResult>();

    public virtual ICollection<UserXRoleResult> UserXRoles { get; set; } = new List<UserXRoleResult>();
}