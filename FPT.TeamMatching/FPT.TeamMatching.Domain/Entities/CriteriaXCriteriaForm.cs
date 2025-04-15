using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class CriteriaXCriteriaForm: BaseEntity
    {
        public Guid? CriteriaFormId { get; set; }

        public Guid? CriteriaId { get; set; }

        public virtual CriteriaForm? CriteriaForm { get; set; }

        public virtual Criteria? Criteria { get; set; }
    }
}
