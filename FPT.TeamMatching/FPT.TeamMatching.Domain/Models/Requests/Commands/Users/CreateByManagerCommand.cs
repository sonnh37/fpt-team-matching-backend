using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Users;

public class CreateByManagerCommand : CreateCommand
{
    public string Email { get; set; }
    public string Code {get;set;}
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public Department? Department { get; set; } = Enums.Department.HoChiMinh;
    public string? Phone { get; set; }
}

// var user = new User
// {
//     Email = email,
//     Code = code,
//     FirstName = firstname,
//     LastName = lastname,
//     Department = Department.HoChiMinh,
//     Username = code.ToLower(),
//     ProfileStudent = new ProfileStudent
//     {
//         UserId = null,
//         SemesterId = upComingSemester.Id,
//         IsQualifiedForAcademicProject = true
//     },
//     UserXRoles = new List<UserXRole>()
//     {
//         new UserXRole
//         {
//             UserId = null,
//             RoleId = roleStudent.Id,
//         }
//     }
// };