using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class ProfileStudentRepository : BaseRepository<ProfileStudent>, IProfileStudentRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public ProfileStudentRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileStudent?> GetProfileByUserId(Guid userId)
    {
        var queryable = GetQueryable(m => m.UserId == userId).Include(m => m.Specialty);
        
        return await queryable.SingleOrDefaultAsync();
    }

    public Task<List<ProfileStudent>> GetProfileByUserIds(List<Guid> guids)
    {
        Guid?[] nullableGuids = guids.Cast<Guid?>().ToArray();
        var queryable = GetQueryable().Where(x => nullableGuids.Contains(x.UserId));
        
        return queryable.ToListAsync();
    }
}