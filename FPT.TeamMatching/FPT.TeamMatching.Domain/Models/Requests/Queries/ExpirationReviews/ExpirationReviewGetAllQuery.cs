using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.ExpirationReviews
{
    public class ExpirationReviewGetAllQuery: GetQueryableQuery
    {
        public Guid? SemesterId { get; set; }

        public int Number { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}
