using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class Criteria : BaseEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ValueType { get; set; }

        public virtual ICollection<CriteriaXCriteriaForm> CriteriaXCriteriaForms { get; set; } = new List<CriteriaXCriteriaForm>();

        public virtual ICollection<AnswerCriteria> AnswerCriterias { get; set; } = new List<AnswerCriteria>();
    }
}
