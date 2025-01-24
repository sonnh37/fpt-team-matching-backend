using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.ProjectActivities
{
    public class ProjectActivityCreateCommand: CreateCommand
    {
        public Guid? ProjectId { get; set; }

        public string? Content { get; set; }
    }
}
