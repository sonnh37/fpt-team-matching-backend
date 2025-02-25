using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class ProfileStudentRepository : BaseRepository<ProfileStudent>, IProfileStudentRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public ProfileStudentRepository(IMapper mapper, FPTMatchingDbContext dbContext) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileStudent> GetProfileByUserId(Guid userId)
    {
        return await _dbContext.ProfileStudents.FirstOrDefaultAsync(x => x.UserId == userId);
    }
}