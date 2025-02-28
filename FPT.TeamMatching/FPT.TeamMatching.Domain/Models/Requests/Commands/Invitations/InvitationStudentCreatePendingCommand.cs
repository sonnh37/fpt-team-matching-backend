using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations
{
    public class InvitationStudentCreatePendingCommand: CreateCommand
    {
        public Guid? ProjectId { get; set; }

        public string? Content { get; set; }
    }
}
