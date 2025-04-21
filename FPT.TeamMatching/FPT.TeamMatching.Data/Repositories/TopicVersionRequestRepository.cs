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
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories
{
    public class TopicVersionRequestRepository : BaseRepository<TopicVersionRequest>, ITopicVersionRequestRepository
    {
        public TopicVersionRequestRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<TopicVersionRequest>> GetByRole(string role)
        {
            var queryable = await GetQueryable()
                .Include(x => x.TopicVersion)
                .ThenInclude(x => x.Topic)
                .ThenInclude(x => x.IdeaVersion)
                .ThenInclude(x => x.Idea)
                .Where(x => x.Role == role).ToListAsync();
            return queryable;
        }
    }
}
