﻿using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Idea : BaseEntity
{
    public Guid? OwnerId { get; set; }

    public Guid? StageIdeaId { get; set; }

    public Guid? MentorId { get; set; }

    public Guid? SubMentorId { get; set; }

    public Guid? SpecialtyId { get; set; }

    public IdeaType? Type { get; set; }

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

    public int MaxTeamSize { get; set; }

    public virtual User? Owner { get; set; }

    public virtual User? Mentor { get; set; }
    
    public virtual User? SubMentor { get; set; }
    
    public virtual Specialty? Specialty { get; set; }

    public virtual Project? Project { get; set; }

    public virtual StageIdea? StageIdea { get; set; }

    public virtual ICollection<IdeaRequest> IdeaRequests { get; set; } = new List<IdeaRequest>();
    
    public virtual ICollection<IdeaHistory> IdeaHistories { get; set; } = new List<IdeaHistory>();

    public virtual ICollection<MentorIdeaRequest> MentorIdeaRequests { get; set; } = new List<MentorIdeaRequest>();
}