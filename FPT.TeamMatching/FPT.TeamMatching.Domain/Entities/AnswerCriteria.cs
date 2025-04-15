using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class AnswerCriteria: BaseEntity
    {
        public Guid? IdeaRequestId { get; set; }

        public Guid? CriteriaId { get; set; }

        public string? Value { get; set; }

        public virtual IdeaRequest? IdeaRequest { get; set; }

        public virtual Criteria? Criteria { get; set; }
    }
}
