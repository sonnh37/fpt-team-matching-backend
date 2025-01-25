using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class TeamMemberResult : BaseResult
    {
        public Guid? ProjectId { get; set; }

        public Guid? UserId { get; set; }

        public TeamMemberRole? Role { get; set; }

        public DateTimeOffset? JoinDate { get; set; }

        public DateTimeOffset? LeaveDate { get; set; }

    } 
}
