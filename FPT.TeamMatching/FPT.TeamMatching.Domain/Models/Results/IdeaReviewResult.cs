using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaReviewResult: BaseResult
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
