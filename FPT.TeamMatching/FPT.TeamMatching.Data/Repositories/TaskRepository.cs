using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPT.TeamMatching.Data.Repositories
{
    public class TaskRepository : BaseRepository<Domain.Entities.Task>, ITaskRepository
    {
        public TaskRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
