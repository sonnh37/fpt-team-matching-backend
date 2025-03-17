using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas
{
    public class IdeaUpdateCommand : UpdateCommand
    {
        public Guid? OwnerId { get; set; }

        public Guid? StageIdeaId { get; set; }

        public Guid? MentorId { get; set; }

        public Guid? SubMentorId { get; set; }

        public IdeaType? Type { get; set; }
    
        public Guid? SpecialtyId { get; set; }

        public string? IdeaCode { get; set; }
    
        public string? Description { get; set; }
    
        public string? Abbreviations { get; set; }

        public string? VietNamName { get; set; }

        public string? EnglishName { get; set; }

        public string? File { get; set; }

        public IdeaStatus? Status { get; set; }

        public bool IsExistedTeam { get; set; }
        
        public bool IsEnterpriseTopic { get; set; }
        
        public string? EnterpriseName { get; set; }

        public int? MaxTeamSize { get; set; }
    }
}
