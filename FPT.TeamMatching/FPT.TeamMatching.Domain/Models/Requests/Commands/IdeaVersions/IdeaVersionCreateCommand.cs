using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersions
{
    public class IdeaVersionCreateCommand: CreateCommand
    {
        public Guid? IdeaId { get; set; }

        public Guid? StageIdeaId { get; set; }

        public int? Version { get; set; }
    }
}
