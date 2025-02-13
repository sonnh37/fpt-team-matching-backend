using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class UserXRoleResult : BaseResult
{
    public Guid? UserId { get; set; }
    
    public Guid? RoleId { get; set; }
    
    public RoleResult? Role { get; set; }
    
    public UserResult? User { get; set; } 
}