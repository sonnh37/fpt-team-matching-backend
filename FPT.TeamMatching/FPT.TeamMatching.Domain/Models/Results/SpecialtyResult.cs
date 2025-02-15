using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class SpecialtyResult: BaseResult
    {
        public Guid? ProfessionId { get; set; }

        public string? SpecialtyName { get; set; }
    }
}
