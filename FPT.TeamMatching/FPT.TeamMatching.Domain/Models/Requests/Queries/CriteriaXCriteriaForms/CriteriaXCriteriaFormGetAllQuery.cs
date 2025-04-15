using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.CriteriaXCriteriaForms
{
    public class CriteriaXCriteriaFormGetAllQuery: GetQueryableQuery
    {
        public Guid? CriteriaFormId { get; set; }

        public Guid? CriteriaId { get; set; }
    }
}
