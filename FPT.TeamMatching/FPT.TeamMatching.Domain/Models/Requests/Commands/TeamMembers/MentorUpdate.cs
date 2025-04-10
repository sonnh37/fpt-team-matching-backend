﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers
{
    public class MentorUpdate: UpdateCommand
    {
        public MentorConclusionOptions MentorConclusion { get; set; }

        public string? Attitude { get; set; }

        public string? Note {get;set;}
    }
}
