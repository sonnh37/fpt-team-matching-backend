using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Topics
{
    public class TopicCreateCommand: CreateCommand
    {
        public Guid? IdeaVersionId { get; set; }

        public string? TopicCode { get; set; }
    }
}
