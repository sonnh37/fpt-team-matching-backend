using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.AnswerCriterias
{
    public class AnswerCriteriaForLecturerRespond
    {
        public Guid? CriteriaId { get; set; }

        public string? Value { get; set; }
    }
}
