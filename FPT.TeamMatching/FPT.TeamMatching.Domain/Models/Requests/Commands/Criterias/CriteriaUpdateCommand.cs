using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Criterias
{
    public class CriteriaUpdateCommand: UpdateCommand
    {
        public string? Question { get; set; }

        public CriteriaValueType? ValueType { get; set; }
    }
}
