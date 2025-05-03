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
        
        public async Task<Semester?> GetBeforeSemester()
        {
            var now = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);

            // Lấy kì học gần nhất đã kết thúc trước ngày hiện tại
            return await GetQueryable()
                .Where(e => DateOnly.FromDateTime(e.EndDate.Value.DateTime) < now)
                .OrderByDescending(e => e.EndDate)
                .FirstOrDefaultAsync();
        }
        public async Task<Semester?> GetSemesterByStageIdeaId(Guid stageIdeaId)
        {
            var s = await GetQueryable()
                .Where(si => si.StageIdeas.Any(e => e.Id == stageIdeaId))
                .FirstOrDefaultAsync();

            return s;
        }

        public async Task<Semester?> GetUpComingSemester()
        {
            var now = DateTime.UtcNow;
            var semester = await GetQueryable().Where(e => e.StartDate >  now)
                                        .OrderBy(e => e.StartDate)
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

        public Task<Semester?> GetSemesterStartToday()
        {
            var queryable = GetQueryable();

            var s = queryable.Where(e => e.IsDeleted == false &&
                                        e.StartDate.Value.Day == DateTime.UtcNow.Day)
                                .FirstOrDefaultAsync();

            return s;
        }
    }
}
