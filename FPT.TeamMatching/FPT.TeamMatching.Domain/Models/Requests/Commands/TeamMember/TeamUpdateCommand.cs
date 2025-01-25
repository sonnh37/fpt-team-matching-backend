using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMember
{
    public class TeamUpdateCommand : UpdateCommand
    {
        public Guid? ProjectId { get; set; }

        public Guid? UserId { get; set; }

        public TeamMemberRole? Role { get; set; }

        public DateTimeOffset? JoinDate { get; set; }

        public DateTimeOffset? LeaveDate { get; set; }

    }
}
