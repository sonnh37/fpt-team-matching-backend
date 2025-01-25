﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects
{
    public class ProjectCreateCommand: CreateCommand
    {
        public Guid? LeaderId { get; set; }

        public string? TeamName { get; set; }

        public string? Name { get; set; }

        public ProjectType? Type { get; set; }

        public string? Description { get; set; }

        public ProjectStatus? Status { get; set; }

        public int? TeamSize { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }
    }
}
