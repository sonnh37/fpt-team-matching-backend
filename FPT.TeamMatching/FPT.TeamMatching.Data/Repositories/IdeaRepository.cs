using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

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
                                        .Include(e => e.StageIdea)
                                        .ToListAsync();
        return ideas;
    }
    
    public async Task<Idea?> GetLatestIdeaByUserAndStatus(Guid userId, IdeaStatus status)
    {
        return await GetQueryable<Idea>()
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync(e => e.OwnerId == userId && e.Status == status);
    }
    
    public async Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status)
    {
        var ideas = await _dbContext.Ideas.Where(e => e.OwnerId == userId
             && e.Status == status)
            .OrderByDescending(m => m.CreatedDate)
            .Include(m => m.StageIdea)
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


    public async Task<List<CustomIdeaResultModel>> GetCustomIdea(Guid semesterId, int reviewNumber)
    {
        // Use async query execution and optimize the LINQ query
        return await GetQueryable()
            .Where(x => x.StageIdea.SemesterId == semesterId)
            .Where(x => x.Project.Reviews.Any(review => review.Number == reviewNumber))
            .Select(y => new CustomIdeaResultModel
            {
                IdeaId = y.Id,
                TeamCode = y.Project.TeamCode,
                IdeaCode = y.IdeaCode,
                Review = y.Project.Reviews
                    .Where(review => review.Number == reviewNumber)
                    .Select(review => new ReviewUpdateCommand
                    {
                        Id = review.Id,     
                        Number = review.Number,  
                        Description = review.Description,
                        Reviewer1 = review.Reviewer1Id,
                        Reviewer2 = review.Reviewer2Id,
                        FileUpload = review.FileUpload,
                        ProjectId = review.ProjectId,
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
}