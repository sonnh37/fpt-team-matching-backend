using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Specialties
{
    public class SpecialtyCreateCommand: CreateCommand
    {
        public Guid? ProfessionId { get; set; }

        public string? SpecialtyName { get; set; }
    }
}
