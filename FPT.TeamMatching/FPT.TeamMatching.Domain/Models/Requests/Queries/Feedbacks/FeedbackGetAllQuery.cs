using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Feedbacks
{
    public class FeedbackGetAllQuery : GetQueryableQuery
    {
        public Guid? ReviewId { get; set; }

        public string? Content { get; set; }

        public string? Description { get; set; }

        public string? FileUpload { get; set; }
    }
}
