using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersions
{
    public class StudentUpdateIdeaCommand : CreateCommand
    {
        public Guid? IdeaId { get; set; }

        public string? FileUpdate { get; set; }

        public int ReviewStage { get; set; }
        public string? Note { get; set; }
    }
}
