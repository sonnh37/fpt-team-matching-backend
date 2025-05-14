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

        public async Task<List<CapstoneSchedule>> GetByProjectId(Guid projectId)
        {
            var capstoneSchedules = await _context.CapstoneSchedules.Where(e => e.IsDeleted == false &&
                                                                     e.ProjectId == projectId)
                                                                .Include(e => e.Project)
                                                                    .ThenInclude(e => e.Topic)
                                                                .Include(x => x.Project).ThenInclude(x => x.TeamMembers).ThenInclude(x => x.User)
                                                                .ToListAsync();  
            return capstoneSchedules;
        }

        public async Task<List<CapstoneSchedule>?> GetBySemesterIdAndStage(Guid semesterId, int stage)
        {
            var capstoneSchedules = await _context.CapstoneSchedules.Where(e => e.IsDeleted == false &&
                                                                    e.Stage == stage &&
                                                                    e.Project != null &&
                                                                    e.Project.Topic != null &&
                                                                    e.Project.Topic != null &&
                                                                    e.Project.Topic.StageTopic != null &&
                                                                    e.Project.Topic.StageTopic.Semester != null &&
                                                                    e.Project.Topic.StageTopic.Semester.Id == semesterId
                                                                    )
                                                                .Include(e => e.Project)
                                                                    .ThenInclude(e => e.Topic)
                                                                    .ThenInclude(e => e.Mentor)
                                                                .Include(e => e.Project)
                                                                    .ThenInclude(e => e.Topic)
                                                                    .ThenInclude(e => e.SubMentor)
                                                                .ToListAsync();
            return capstoneSchedules;
        }
    }
}
