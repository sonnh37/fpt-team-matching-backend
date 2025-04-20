using FPT.TeamMatching.Data.Context;
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
    public class TopicVersionRepository : BaseRepository<TopicVersion>, ITopicVersionRepository
    {
        private readonly FPTMatchingDbContext _dbContext;
        public TopicVersionRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TopicVersion>> GetAllByIdeaId(Guid ideaId)
        {
           //return await GetQueryable().Where(h => h.IdeaId == ideaId).ToListAsync();
           return await GetQueryable().Where(h => h.Topic.IdeaVersionId == ideaId).ToListAsync();
        }
    }
}
