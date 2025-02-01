using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class VerifySemesterRepository : BaseRepository<VerifySemester>, IVerifySemesterRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    public VerifySemesterRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }

    public async Task<VerifySemester> FindByUserId(Guid semesterId)
    {
        return await _dbContext.VerifySemesters.FirstOrDefaultAsync(x => x.UserId == semesterId);
    }
}