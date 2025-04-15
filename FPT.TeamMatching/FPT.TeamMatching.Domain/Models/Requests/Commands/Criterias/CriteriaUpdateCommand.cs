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
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ValueType { get; set; }
    }
}
