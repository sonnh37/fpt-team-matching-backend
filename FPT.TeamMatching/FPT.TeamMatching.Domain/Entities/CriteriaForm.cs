﻿using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class CriteriaForm: BaseEntity
    {
        public string? Title { get; set; }

        public virtual ICollection<CriteriaXCriteriaForm> CriteriaXCriteriaForms { get; set; } = new List<CriteriaXCriteriaForm>();

        public virtual ICollection<TopicRequest> TopicRequests { get; set; } = new List<TopicRequest>();
        
        public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();

    }
}
