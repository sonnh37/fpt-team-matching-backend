using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersions
{
    public class IdeaVersionUpdateCommand: UpdateCommand
    {
        public Guid? IdeaId { get; set; }

        public Guid? StageIdeaId { get; set; }

        public int? Version { get; set; }

        public string? VietNamName { get; set; }

        public string? EnglishName { get; set; }

        public string? Description { get; set; }

        public string? Abbreviations { get; set; }

        public string? EnterpriseName { get; set; }

        public int? TeamSize { get; set; }

        public string? File { get; set; }
    }
}
