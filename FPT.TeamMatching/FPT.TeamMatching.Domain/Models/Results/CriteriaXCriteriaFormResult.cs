
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class CriteriaXCriteriaFormResult: BaseResult
    {
        public Guid? CriteriaFormId { get; set; }

        public Guid? CriteriaId { get; set; }

        public CriteriaFormResult? CriteriaForm { get; set; }

        public CriteriaResult? Criteria { get; set; }
    }
}
