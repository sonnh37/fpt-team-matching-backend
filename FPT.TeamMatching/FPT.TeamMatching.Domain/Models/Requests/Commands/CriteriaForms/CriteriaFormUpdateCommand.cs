﻿using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.CriteriaForms
{
    public class CriteriaFormUpdateCommand: UpdateCommand
    {
        public string? Title { get; set; }

    }
}
