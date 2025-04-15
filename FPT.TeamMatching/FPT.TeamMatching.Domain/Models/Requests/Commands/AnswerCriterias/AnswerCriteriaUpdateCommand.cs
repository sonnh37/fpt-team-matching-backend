using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.AnswerCriterias
{
    public class AnswerCriteriaUpdateCommand: UpdateCommand
    {
        public Guid? IdeaRequestId { get; set; }

        public Guid? CriteriaId { get; set; }

        public string? Value { get; set; }
    }
}
