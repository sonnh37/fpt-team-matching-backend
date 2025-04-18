using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class Criteria : BaseEntity
    {
        public string? Question { get; set; }

        public CriteriaValueType? ValueType { get; set; }

        public virtual ICollection<CriteriaXCriteriaForm> CriteriaXCriteriaForms { get; set; } = new List<CriteriaXCriteriaForm>();

        public virtual ICollection<AnswerCriteria> AnswerCriterias { get; set; } = new List<AnswerCriteria>();
    }
}
