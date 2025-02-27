using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaHistories
{
    public class IdeaHistoryGetAllQuery: GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public Guid? CouncilId { get; set; }

        public string? FileUpdate { get; set; }

        public IdeaHistoryStatus? Status { get; set; }

        public int ReviewStage { get; set; }
    }
}
