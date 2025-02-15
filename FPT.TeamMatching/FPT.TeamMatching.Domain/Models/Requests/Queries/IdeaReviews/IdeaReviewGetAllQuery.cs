using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaReviews
{
    public class IdeaReviewGetAllQuery : GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
