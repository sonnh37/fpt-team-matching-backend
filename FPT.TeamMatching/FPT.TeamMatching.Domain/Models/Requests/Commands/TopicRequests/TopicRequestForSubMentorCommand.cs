using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TopicRequests
{
    public class TopicRequestForSubMentorCommand
    {
        public Guid? TopicId { get; set; }

        public Guid? ReviewerId { get; set; }
    }
}
