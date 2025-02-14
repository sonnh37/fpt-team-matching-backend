using CloudinaryDotNet.Actions;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaResult: Bases.BaseResult
    {
        public Guid? UserId { get; set; }

        public Guid? SemesterId { get; set; }

        public Guid? SubMentorId { get; set; }

        public IdeaType? Type { get; set; }

        public string? Title { get; set; }

        public string? IdeaCode { get; set; }

        public string? VietNamName { get; set; }

        public string? EnglishName { get; set; }

        public string? Major { get; set; }

        public string? File { get; set; }

        public string? Description { get; set; }

        public ProjectStatus? Status { get; set; }

        public bool? IsExistedTeam { get; set; }

        public int? MaxTeamSize { get; set; }
    }
}
