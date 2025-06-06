﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Criterias
{
    public class CriteriaGetAllQuery: GetQueryableQuery
    {
        public string? Question { get; set; }

        public CriteriaValueType? ValueType { get; set; }
    }
}
