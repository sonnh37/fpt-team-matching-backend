﻿using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class CriteriaFormResult: BaseResult
    {
        public string? Title { get; set; }

        public virtual ICollection<CriteriaXCriteriaFormResult> CriteriaXCriteriaForms { get; set; } = new List<CriteriaXCriteriaFormResult>();

        public virtual ICollection<IdeaVersionRequestResult> IdeaRequests { get; set; } = new List<IdeaVersionRequestResult>();

        public virtual ICollection<SemesterResult> Semesters { get; set; } = new List<SemesterResult>();

    }
}
