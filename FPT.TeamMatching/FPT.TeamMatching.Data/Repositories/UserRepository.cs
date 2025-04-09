using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities.Filters;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using Role = FPT.TeamMatching.Domain.Entities.Role;

namespace FPT.TeamMatching.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetUserByUsernameOrEmail(string key)
    {
        key = key.Trim().ToLower();
        var queyable = GetQueryable();
        queyable = IncludeHelper.Apply(queyable);

        return await queyable
            .Where(entity => !entity.IsDeleted)
            .Where(e => e.Email!.ToLower().Trim() == key || e.Username!.ToLower().Trim() == key)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetById(Guid id)
    {
        var queryable = GetQueryable(x => x.Id == id);
        queryable = queryable.Include(e => e.UserXRoles).ThenInclude(e => e.Role);
        var entity = await queryable.FirstOrDefaultAsync();

        return entity;
    }

    public async Task<List<User>> GetThreeCouncilsForIdeaRequest(Guid ideaId)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.UserXRoles).ThenInclude(m => m.Role);

        // Lấy thông tin idea để lấy mentor
        var idea = await GetQueryable<Idea>()
            .Where(i => i.Id == ideaId)
            .Select(i => new { i.MentorId, i.SubMentorId })
            .FirstOrDefaultAsync();

        if (idea == null)
        {
            throw new Exception("Idea not found");
        }

        var councils = await queryable
            .Where(u => u.UserXRoles.Any(m => m.Role != null && m.Role.RoleName == "Council"))
            .Select(u => new
            {
                Council = u,
                ApprovedCount = GetQueryable<IdeaRequest>()
                    .Count(ir => ir.ReviewerId == u.Id && ir.Status == IdeaRequestStatus.Approved && ir.Role == "Council")
            })
            .OrderBy(x => x.ApprovedCount)
            .Select(x => x.Council)
            .ToListAsync();

        // Lọc bỏ những council trùng với mentor hoặc sub-mentor
        councils = councils.Where(c => c.Id != idea.MentorId && c.Id != idea.SubMentorId).Take(3).ToList();

        return councils;
    }

    public async Task<User?> GetByEmail(string keyword)
    {
        var queryable = GetQueryable();

        var user = await queryable.Where(e => e.Email!.ToLower() == keyword.ToLower())
            .SingleOrDefaultAsync();

        return user;
    }

    public async Task<User?> GetByUsername(string username)
    {
        var queryable = GetQueryable();

        var user = await queryable.Where(e => e.Username!.ToLower() == username.ToLower())
            .SingleOrDefaultAsync();

        return user;
    }

    public async Task<List<PartnerInfoResult>> GetAllUsersWithNameOnly()
    {
        var users = await GetQueryable()
            .Select(x => new PartnerInfoResult
            {
                Id = x.Id.ToString(),
                LastName = x.LastName,
                FirstName = x.FirstName,
            })
            .ToListAsync();

        return users;
    }

    public async Task<User?> GetReviewerByMatchingEmail(string keyword)
    {
        var queryable = GetQueryable();
        var reviewer = await queryable.Where(e => e.Email != null
                                            && e.Email.Substring(0, e.Email.IndexOf("@")).ToLower() == keyword.ToLower()
                                            && e.UserXRoles.Any(e => e.Role != null
                                            && e.Role.RoleName == "Reviewer"))
                                        .FirstOrDefaultAsync();
        return reviewer;
    }

    public async Task<List<UserIdEmailResult>> GetAllReviewerIdAndUsername()
    {
        var result = await GetQueryable()
            .Where(e => e.UserXRoles.Any(e => e.Role.RoleName == "Reviewer"))
            .Select(x =>
                new UserIdEmailResult
                {
                    Username = x.Username.ToLower(),
                    Id = x.Id,
                    Code = x.Code.ToLower(),
                }).Distinct().ToListAsync();
        return result;
    }

    public async Task<List<User>?> GetStudentDoNotHaveTeam()
    {
        var students = await GetQueryable().Where(e => e.IsDeleted == false &&
                                            e.UserXRoles.Any(e => e.Role.RoleName == "Student") &&
                                            (// Chưa có nhóm (không có TeamMembers nào)
                                            !e.TeamMembers.Any() ||
                                            // Có nhóm nhưng tất cả status đều "Fail"
                                            //e.TeamMembers.All(tm => tm.Status == TeamMemberStatus.Failed)))
                                            e.TeamMembers.All(tm => tm.Status == TeamMemberStatus.Fail2)))
                                            .ToListAsync();
        return students;
    }
}