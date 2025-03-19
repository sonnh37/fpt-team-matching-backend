using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.StageIdeas
{
    public class StageIdeaGetAllQuery: GetQueryableQuery
    {
        public Guid? SemesterId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public DateTimeOffset ResultDate { get; set; }
        
        public int StageNumber  { get; set; }

    }
}
