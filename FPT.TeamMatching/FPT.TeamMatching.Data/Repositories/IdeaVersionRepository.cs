using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Data.Repositories
{
    public class IdeaVersionRepository : BaseRepository<IdeaVersion>, IIdeaVersionRepository
    {
        private readonly FPTMatchingDbContext _context;

        public IdeaVersionRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<IdeaVersion>> GetIdeaVersionsByIdeaId(Guid ideaId)
        {
            var ideaVersions = await _context.IdeaVersions.Where(e => e.IsDeleted == false &&
                    e.IdeaId == ideaId)
                .ToListAsync();
            return ideaVersions;
        }

    }
}