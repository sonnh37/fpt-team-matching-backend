﻿using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Data.Repositories
{
    public class CriteriaXCriteriaFormRepository : BaseRepository<CriteriaXCriteriaForm>, ICriteriaXCriteriaFormRepository
    {
        public CriteriaXCriteriaFormRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
