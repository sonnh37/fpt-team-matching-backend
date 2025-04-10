﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests
{
    public class IdeaRequestGetAllQuery : GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }
    
        public string? Content { get; set; }
    
        public IdeaRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
        
        public string? Role { get; set; }
    }
}
