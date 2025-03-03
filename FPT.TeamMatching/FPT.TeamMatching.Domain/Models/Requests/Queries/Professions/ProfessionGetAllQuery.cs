using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Professions
{
    public class ProfessionGetAllQuery: GetQueryableQuery
    {
        public string? ProfessionName { get; set; }
    }
}
