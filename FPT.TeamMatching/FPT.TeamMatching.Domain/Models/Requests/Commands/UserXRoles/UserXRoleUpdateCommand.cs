using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.UserXRoles
{
    public class UserXRoleUpdateCommand: UpdateCommand
    {
        public Guid? UserId { get; set; }

        public Guid? RoleId { get; set; }

        public Guid? SemesterId { get; set; }

        public bool IsPrimary { get; set; }
    }
}
