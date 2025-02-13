using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Role : BaseEntity
{
    public string? RoleName { get; set; }
    
    public virtual ICollection<UserXRole> UserXRoles { get; set; } = new List<UserXRole>();
}