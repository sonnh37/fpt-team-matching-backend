﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas
{
    public class IdeaGetAllQuery : GetQueryableQuery
    {
        public Guid? OwnerId { get; set; }

        public Guid? MentorId { get; set; }

        public Guid? SubMentorId { get; set; }

        public Guid? SpecialtyId { get; set; }

        public IdeaType? Type { get; set; }

        public IdeaStatus? Status { get; set; }

        public bool IsExistedTeam { get; set; }

        public bool IsEnterpriseTopic { get; set; }
    }
}
