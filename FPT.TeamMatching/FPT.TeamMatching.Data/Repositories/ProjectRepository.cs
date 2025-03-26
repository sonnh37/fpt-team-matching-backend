using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    private readonly FPTMatchingDbContext _context;

    public ProjectRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<Project?> GetProjectByUserIdLogin(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId && e.LeaveDate == null && e.IsDeleted == false)
            .FirstOrDefaultAsync();
        if (teamMember != null)
        {
            var project = await _context.Projects.Where(e => e.Id == teamMember.ProjectId)
                .Include(e => e.TeamMembers)
                .ThenInclude(e => e.User).Include(e => e.Idea)
                .ThenInclude(e => e.Specialty)
                .ThenInclude(e => e.Profession)
                .Include(e => e.Invitations.Where(e1 => e1.Status != null && e1.Type != null && e1.SenderId != null
                                                        && (e1.Status.Value == InvitationStatus.Pending
                                                            && e1.Type.Value == InvitationType.SentByStudent
                                                            && e1.SenderId == userId)))
                .FirstOrDefaultAsync();
            if (project != null)
            {
                return project;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public async Task<List<Project>?> GetProjectsStartingNow()
    {
        var today = DateTime.UtcNow.Date;
        var project = await _context.Projects
            .Where(p => p.IsDeleted == false &&
                        p.Idea != null &&
                        p.Idea.StageIdea != null &&
                        p.Idea.StageIdea.Semester != null &&
                        p.Idea.StageIdea.Semester != null &&
                        p.Idea.StageIdea.Semester.StartDate != null &&
                        p.Idea.StageIdea.Semester.StartDate.Value.UtcDateTime.Date == today)
            .ToListAsync();
        return project;
    }

    public async Task<Project?> GetProjectByCode(string code)
    {
        var project = await _context.Projects.Where(e => e.TeamCode == code).FirstOrDefaultAsync();
        return project;
    }

    public async Task<Project?> GetProjectOfUserLogin(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.IsDeleted == false &&
                                                e.UserId == userId &&
                                                e.Status == TeamMemberStatus.InProgress)
                                            .FirstOrDefaultAsync();
        if (teamMember != null)
        {
            var project = await _context.Projects.Where(e => e.IsDeleted ==false &&
                                                e.Id == teamMember.ProjectId)
                                            .FirstOrDefaultAsync();
            return project;
        }
        return null;
    }
}