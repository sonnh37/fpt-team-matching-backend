using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
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

    public async Task<(List<User>, int)> GetAllByCouncilWithIdeaRequestPending(UserGetAllQuery query)
    {
        var queryable = GetQueryable();
        queryable = queryable.Include(m => m.UserXRoles).ThenInclude(m => m.Role);

        queryable = queryable.Where(m =>
            m.UserXRoles.Any(uxr => uxr.Role != null && uxr.Role.RoleName == "Council"));

        queryable = queryable
            .Where(m => m.IdeaRequestOfReviewers.Any(n => n.Status == IdeaVersionRequestStatus.Pending))
            .Include(m => m.IdeaRequestOfReviewers
                .Where(n => n.Status == IdeaVersionRequestStatus.Pending));

        if (query.Department.HasValue)
        {
            queryable = queryable.Where(m =>
                m.Department == query.Department);
        }

        if (!string.IsNullOrEmpty(query.EmailOrFullname))
        {
            queryable = queryable.Where(m =>
                m.LastName != null && m.FirstName != null &&
                ((m.Email != null && m.Email.Contains(query.EmailOrFullname.Trim().ToLower())) ||
                 (m.LastName.Trim().ToLower() + " " + m.FirstName.Trim().ToLower()).Contains(query.EmailOrFullname
                     .Trim().ToLower()))
            );
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        if (query.IsPagination)
        {
            // Tổng số count sau khi  filter khi chưa lọc trang
            var totalOrigin = queryable.Count();
            // Sắp sếp
            queryable = Sort(queryable, query);
            // Lọc trang
            var results = await GetQueryablePagination(queryable, query).ToListAsync();

            return (results, totalOrigin);
        }
        else
        {
            queryable = Sort(queryable, query);
            var results = await queryable.ToListAsync();
            return (results, results.Count);
        }
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
                ApprovedCount = GetQueryable<IdeaVersionRequest>()
                    .Count(ir =>
                        ir.ReviewerId == u.Id && ir.Status == IdeaVersionRequestStatus.Approved && ir.Role == "Council")
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

        var user = await queryable.Where(e => e.Email != null && e.Email.ToLower().Trim() == keyword.ToLower().Trim())
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
                                                  && e.Email.Substring(0, e.Email.IndexOf("@")).ToLower() ==
                                                  keyword.ToLower()
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
                                                       ( // Chưa có nhóm (không có TeamMembers nào)
                                                           !e.TeamMembers.Any() ||
                                                           // Có nhóm nhưng tất cả status đều "Fail"
                                                           //e.TeamMembers.All(tm => tm.Status == TeamMemberStatus.Failed)))
                                                           e.TeamMembers.All(tm =>
                                                               tm.Status == TeamMemberStatus.Fail2)))
            .ToListAsync();
        return students;
    }
    
    public async Task<List<EmailSuggestionModels>> GetAllEmailSuggestions(string email)
    {
       var result = await GetQueryable()
           .Where(e => e.IsDeleted == false && e.Email.Contains(email))
           .Select(x => new EmailSuggestionModels
           {
               Email = x.Email,
               UserId = x.Id,
           })
           .Take(5)
           .ToListAsync();
       return result;
    }
}