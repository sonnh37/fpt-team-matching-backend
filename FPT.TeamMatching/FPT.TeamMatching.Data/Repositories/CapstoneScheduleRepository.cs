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
    public class CapstoneScheduleRepository : BaseRepository<CapstoneSchedule>, ICapstoneScheduleRepository
    {
        private readonly FPTMatchingDbContext _context;
        public CapstoneScheduleRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public Task<List<CapstoneSchedule>?> GetBySemesterIdAndStage(Guid semesterId, int stage)
        {
            var capstoneSchedules = _context.CapstoneSchedules.Where(e => e.IsDeleted == false &&
                                                                    e.Stage == stage &&
                                                                    e.Project.Idea.StageIdea.Semester.Id == semesterId)
                                                                .Include(e => e.Project)
                                                                .ToListAsync();
            return capstoneSchedules;
        }
    }
}
