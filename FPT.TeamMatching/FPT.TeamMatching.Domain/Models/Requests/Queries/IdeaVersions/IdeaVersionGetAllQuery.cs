using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersions
{
    public class IdeaVersionGetAllQuery: GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public Guid? StageIdeaId { get; set; }

        public int? Version { get; set; }
    }
}
