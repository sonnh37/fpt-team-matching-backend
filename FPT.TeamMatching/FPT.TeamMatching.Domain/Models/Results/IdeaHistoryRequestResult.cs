using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaHistoryRequestResult: BaseResult
    {
        public Guid? IdeaHistoryId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }

        public IdeaHistoryRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
