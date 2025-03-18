using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRepository : BaseRepository<Idea>, IIdeaRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    public IdeaRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<Idea>> GetIdeasByUserId(Guid userId)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId)
                                        .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
                                        .ToListAsync();
        return ideas;
    }

    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
             && e.Status == status)
            .OrderByDescending(m => m.CreatedDate)
            .Include(e => e.IdeaRequests).ThenInclude(e => e.Reviewer)
            .ToListAsync();

        return ideas;
    }

    public async Task<int> MaxNumberOfSemester(Guid semesterId)
    {
        // # Lỗi do thay đổi db
        // var maxNumber = await _dbContext.Ideas
        //                .Where(i => i.SemesterId == semesterId)
        //             .Select(i => Regex.Match(i.IdeaCode, @"\d+$").Value) // Lấy phần số cuối
        //             .Select(x => int.Parse(x)) // Chuyển thành số nguyên
        //             .DefaultIfEmpty(0) // Nếu không có Idea nào, giá trị mặc định là 0
        //             .MaxAsync();
        // return maxNumber;
        return 0;
    }

    
}