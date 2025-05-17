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
    public class StageTopicRepository : BaseRepository<StageTopic>, IStageTopicRepository
    {
        private readonly ISemesterRepository _semesterRepository;

        public StageTopicRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) :
            base(dbContext)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<StageTopic?> GetByStageNumberAndSemester(int number, Guid semesterId)
        {
            var stageIdea = await GetQueryable()
                .FirstOrDefaultAsync(x => x.SemesterId == semesterId && x.StageNumber == number);

            return stageIdea;
        }

        public async Task<StageTopic?> GetCurrentStageTopic()
        {
            return await GetQueryable()
                .Where(e => e.IsDeleted == false &&
                        e.StartDate.LocalDateTime.Date <= DateTime.Now.Date &&
                        DateTime.Now.Date <= e.EndDate.LocalDateTime.Date )
                .Include(e => e.Semester)
                .FirstOrDefaultAsync();
        }

        public async Task<StageTopic?> GetCurrentStageTopicBySemesterId(Guid semesterId)
        {
            return await GetQueryable()
                .Where(e => e.IsDeleted == false &&
                        e.StartDate.LocalDateTime.Date <= DateTime.Now.Date &&
                        DateTime.Now.Date <= e.EndDate.LocalDateTime.Date &&
                        e.SemesterId == semesterId)
                .Include(e => e.Semester)
                .FirstOrDefaultAsync();
        }
    }
}