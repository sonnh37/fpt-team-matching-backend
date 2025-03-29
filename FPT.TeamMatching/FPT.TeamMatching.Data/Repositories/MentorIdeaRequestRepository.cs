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
    public class MentorIdeaRequestRepository : BaseRepository<MentorIdeaRequest>, IMentorIdeaRequestRepository
    {
        private readonly FPTMatchingDbContext _dbContext;
        public MentorIdeaRequestRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MentorIdeaRequest>?> GetByIdeaId(Guid ideaId)
        {
            var requests = await _dbContext.MentorIdeaRequests.Where(e => e.IsDeleted == false &&
                                                                e.IdeaId == ideaId)
                                                        .ToListAsync();
            return requests;
        }
    }
}
