using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
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
        public string? Question { get; set; }

        public CriteriaValueType? ValueType { get; set; }

        public ICollection<CriteriaXCriteriaFormResult> CriteriaXCriteriaForms { get; set; } = new List<CriteriaXCriteriaFormResult>();

        public ICollection<AnswerCriteriaResult> AnswerCriterias { get; set; } = new List<AnswerCriteriaResult>();
    }
}
