using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class CriteriaResult: BaseResult
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ValueType { get; set; }

        public virtual ICollection<CriteriaXCriteriaFormResult> CriteriaXCriteriaForms { get; set; } = new List<CriteriaXCriteriaFormResult>();

        public virtual ICollection<AnswerCriteriaResult> AnswerCriterias { get; set; } = new List<AnswerCriteriaResult>();
    }
}
