using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Data.Repositories
{
    public class SemesterRepository : BaseRepository<Semester>, ISemesterRepository
    {
        public SemesterRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Semester?> GetPresentSemester()
        {
            var s = await _dbContext.Semesters.Where(e => e.StartDate.Value.LocalDateTime.Date <= DateTime.Now.Date && DateTime.Now.Date <= e.EndDate.Value.LocalDateTime.Date)
                                        .FirstOrDefaultAsync();
            return s;
        }

        public async Task<Semester?> GetSemesterByStageIdeaId(Guid stageIdeaId)
        {
            var s = await _dbContext.StageIdeas
                .Where(si => si.Id == stageIdeaId)
                .Select(si => si.Semester)
                .FirstOrDefaultAsync();

            return s;
        }

        public async Task<Semester?> GetUpComingSemester()
        {
            var now = DateTime.UtcNow;
            var semester = await GetQueryable().Where(e => e.StartDate >  now)
                                        .OrderByDescending(e => e.StartDate)
                                        .FirstOrDefaultAsync();
            return semester;
        }
        
        public async Task<Semester?> GetCurrentSemester()
        {
            var now = DateTime.UtcNow;

            var semester = await GetQueryable()
                .Where(e => e.StartDate <= now && e.EndDate >= now)
                .OrderByDescending(e => e.StartDate)
                .FirstOrDefaultAsync();
            return semester;
        }
    }
}
