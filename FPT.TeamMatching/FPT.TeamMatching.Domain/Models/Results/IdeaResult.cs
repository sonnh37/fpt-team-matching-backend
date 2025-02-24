﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaResult: BaseResult
    {
        public Guid? UserId { get; set; }

        public Guid? SemesterId { get; set; }

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

        public  UserResult? User { get; set; }

        public  UserResult? SubMentor { get; set; }
    
        public  SpecialtyResult? Specialty { get; set; }

        public  ProjectResult? Project { get; set; }

        public  SemesterResult? Semester { get; set; }

        public  ICollection<IdeaRequestResult> IdeaRequests { get; set; } = new List<IdeaRequestResult>();
    }
}
