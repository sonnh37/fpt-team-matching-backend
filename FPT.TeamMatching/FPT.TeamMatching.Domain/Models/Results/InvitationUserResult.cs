using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class InvitationUserResult: BaseResult
    {
        public Guid? ProjectId { get; set; }

        public Guid? SenderId { get; set; }

        public Guid? ReceiverId { get; set; }

        public InvitationUserStatus? Status { get; set; }

        public InvitationUserType? Type { get; set; }

        public string? Content { get; set; }
    }
}
