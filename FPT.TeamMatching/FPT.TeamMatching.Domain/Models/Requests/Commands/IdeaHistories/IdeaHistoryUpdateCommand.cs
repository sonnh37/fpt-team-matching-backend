using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistories
{
    public class IdeaHistoryUpdateCommand: UpdateCommand
    {
        public Guid? IdeaId { get; set; }

        public Guid? CouncilId { get; set; }

        public string? FileUpdate { get; set; }

        public IdeaHistoryStatus? Status { get; set; }

        public string? Comment { get; set; }

        public int ReviewStage { get; set; }
    }
}
