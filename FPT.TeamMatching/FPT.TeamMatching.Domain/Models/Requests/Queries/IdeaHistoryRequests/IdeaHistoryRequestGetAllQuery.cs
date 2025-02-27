using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaHistoryRequests
{
    public class IdeaHistoryRequestGetAllQuery: GetQueryableQuery
    {
        public Guid? IdeaHistoryId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }

        public IdeaHistoryRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
