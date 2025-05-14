using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicOld
{
    public class TopicOldGetAllQuery : GetQueryableQuery
    {
        public Guid? IdeaVersionId { get; set; }

        public string? TopicCode { get; set; }
    }
}
