﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicVersionRequests
{
    public class TopicVersionRequestGetAllQuery: GetQueryableQuery
    {
        public Guid? TopicVersionId { get; set; }

        public Guid? ReviewerId { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public TopicVersionRequestStatus? Status { get; set; }

        public string? Role { get; set; }

        public string? Feedback { get; set; }

    }
}
