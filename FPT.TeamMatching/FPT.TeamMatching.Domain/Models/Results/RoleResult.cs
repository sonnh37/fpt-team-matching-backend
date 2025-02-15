using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class RoleResult : BaseResult
{
    public string? RoleName { get; set; }

    public ICollection<UserXRoleResult> UserXRoles { get; set; } = new List<UserXRoleResult>();
}