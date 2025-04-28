using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Users
{
    public class UserCheckMentorAndSubMentorQuery
    {
        public Guid MentorId { get; set; }

        public Guid? SubMentorId { get; set; }
    }
}
