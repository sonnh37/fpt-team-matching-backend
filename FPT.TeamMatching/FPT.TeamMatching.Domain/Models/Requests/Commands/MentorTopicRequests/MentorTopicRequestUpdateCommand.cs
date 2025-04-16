using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.MentorTopicRequests
{
    public class MentorTopicRequestUpdateCommand: UpdateCommand
    {
        public Guid? ProjectId { get; set; }

        public Guid? TopicId { get; set; }

        public MentorTopicRequestStatus? Status { get; set; }
    }
}
