using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.MentorIdeaRequest
{
    public class MentorIdeaRequestGetAllQuery: GetQueryableQuery
    {
        public Guid? ProjectId { get; set; }

        public Guid? IdeaId { get; set; }

        public MentorIdeaRequestStatus? Status { get; set; }
    }
}
