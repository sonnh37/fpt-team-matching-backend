using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Users;

public class UserCreateCommand : CreateCommand
{
    public Gender? Gender { get; set; }

    public string? Cache { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Avatar { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Department? Department { get; set; }
}