using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.CriteriaForms
{
    public class CriteriaFormGetAllQuery: GetQueryableQuery
    {
        public string? Title { get; set; }

    }
}
