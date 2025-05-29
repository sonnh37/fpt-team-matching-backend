using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Utilities.Filters;
using Microsoft.EntityFrameworkCore;
using Pipelines.Sockets.Unofficial.Arenas;

namespace FPT.TeamMatching.Data.Repositories;

public class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public InvitationRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Invitation?> GetInvitationOfUserByProjectId(Guid projectId, Guid userId)
    {
        var i = await _dbContext.Invitations.Where(e => e.Status != null
                                                        && e.ProjectId == projectId
                                                        && e.SenderId == userId
                                                        && e.Status.Value == InvitationStatus.Pending
                                                        && e.IsDeleted == false)
            .SingleOrDefaultAsync();
        return i;
    }

    public async Task<Invitation?> GetInvitationOfTeamByProjectIdAndMe(Guid projectId, Guid userId)
    {
        var i = await _dbContext.Invitations.Where(e => e.Status != null
                                                        && e.ProjectId == projectId
                                                        && e.ReceiverId == userId
                                                        && e.Status.Value == InvitationStatus.Pending
                                                        && e.IsDeleted == false)
            .SingleOrDefaultAsync();
        return i;
    }
    
    public async Task<List<Invitation>> GetPendingInvitationsForProjectFromOtherSendersAsync(
        Guid senderId, 
        Guid projectId, 
        Guid excludeInvitationId)
    {
        return await _dbContext.Invitations
            .Where(i => i.Status == InvitationStatus.Pending)
            .Where(i => i.Id != excludeInvitationId)
            .Where(i => i.ProjectId == projectId && i.SenderId != senderId)
            .ToListAsync();
    }
   

    public async Task<(List<Invitation>, int)> GetUserInvitationsByType(InvitationGetByTypeQuery query, Guid userId, Guid semesterId)
    {
        var queryable = GetQueryable(m => m.Type == query.Type && m.Project.SemesterId == semesterId);
        queryable = queryable
            .Include(m => m.Project)
            .Include(m => m.Receiver)
            .Include(m => m.Sender);
        queryable = query.Type switch
        {
            // Get ra list đã gửi những ai
            InvitationType.SentByStudent => queryable.Where(m => m.SenderId == userId),

            // Get ra list đã nhận bởi team nào
            InvitationType.SendByTeam => queryable.Where(m => m.ReceiverId == userId),
            _ => queryable
        };

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
    
    public async Task<(List<Invitation>, int)> GetUserInvitationsByStatus(InvitationGetListForUserByStatus query, Guid userId)
    {
        var queryable = GetQueryable(m => m.Status == query.Status);
        queryable = queryable
            .Include(m => m.Project)
            .Include(m => m.Receiver)
            .Include(m => m.Sender);

        queryable = queryable.Where(m => m.SenderId == userId);
        if (query.IsPagination)
        {
            var totalOrigin = queryable.Count();
            queryable = Sort(queryable, query);
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

    public async Task<(List<Invitation>, int)> GetLeaderInvitationsByType(InvitationGetByTypeQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable.Where(m => m.Type == query.Type && m.ProjectId == query.ProjectId)
            .Include(m => m.Project)
            .Include(m => m.Receiver)
            .Include(m => m.Sender);

        if (query.Status != null)
        {
            queryable = queryable.Where(m => m.Status == query.Status);
        }
        
        queryable = query.Type switch
        {
            // Get ra list đã gửi những ai
            InvitationType.SentByStudent => queryable.Where(m => m.ReceiverId == userId),

            // Get ra list đã nhận bởi team nào
            InvitationType.SendByTeam => queryable.Where(m => m.SenderId == userId),
            _ => queryable
        };


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

    public async Task<List<Invitation>> GetInvitationsByStatusAndProjectId(InvitationStatus status, Guid projectId)
    {
        var queryable = GetQueryable();

        var invitations = await queryable.Where(e => e.IsDeleted == false &&
                                                    e.Status == status &&
                                                    e.ProjectId == projectId)
                                        .ToListAsync();      

        return invitations;
    }

    public async Task<List<Invitation>?> GetInvitationsBySenderIdAndStatus(Guid senderId, InvitationStatus status)
    {
        var queryable = GetQueryable();

        var invitations = await queryable.Where(e => e.IsDeleted == false &&
                                                    e.Status == status &&
                                                    e.SenderId == senderId)
                                        .ToListAsync();

        return invitations;
    }
}