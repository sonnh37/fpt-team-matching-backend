﻿using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Role
{
    public class RoleGetAllQuery: GetQueryableQuery
    {
        public string? RoleName { get; set; }

    }
}
