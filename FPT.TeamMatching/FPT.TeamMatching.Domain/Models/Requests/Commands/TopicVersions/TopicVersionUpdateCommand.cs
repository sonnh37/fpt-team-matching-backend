using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersions
{
    public class TopicVersionUpdateCommand : UpdateCommand
    {
        public Guid? TopicId { get; set; }

        public string? FileUpdate { get; set; }

        public TopicVersionStatus? Status { get; set; }

        public string? Comment { get; set; }

        public int ReviewStage { get; set; }
    }
}
