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
    public class StageIdeaRepository : BaseRepository<StageIdea>, IStageIdeaRepositoty
    {
        private readonly ISemesterRepository _semesterRepository;

        public StageIdeaRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) :
            base(dbContext)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<StageIdea?> GetByStageNumberAndSemester(int number, Guid semesterId)
        {
            var stageIdea = await GetQueryable()
                .FirstOrDefaultAsync(x => x.SemesterId == semesterId && x.StageNumber == number);

            return stageIdea;
        }
        
        public async Task<StageIdea?> GetLatestStageIdea()
        {
            return await GetQueryable()
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }
    }
}