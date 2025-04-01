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
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId &&
                                                    e.LeaveDate == null &&
                                                    e.IsDeleted == false)
                                                    .FirstOrDefaultAsync();
        if (teamMember != null)
        {
            var project = await _context.Projects.Where(e => e.Id == teamMember.ProjectId)
                .Include(e => e.TeamMembers)
                .ThenInclude(e => e.User).Include(e => e.Idea)
                .ThenInclude(e => e.Specialty)
                .ThenInclude(e => e.Profession)
                .Include(e => e.Invitations)
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
                                                e.Status == TeamMemberStatus.Pending)
                                            .FirstOrDefaultAsync();
        if (teamMember != null)
        {
            var project = await _context.Projects.Where(e => e.IsDeleted == false &&
                                                e.Id == teamMember.ProjectId)
                                            .FirstOrDefaultAsync();
            return project;
        }
        return null;
    }

    public async Task<int> NumberOfInProgressProjectInSemester(Guid semesterId)
    {
        var number = await _context.Projects.Where(e => e.Status == ProjectStatus.InProgress &&
                                                          e.IsDeleted == false &&
                                                          e.Idea != null &&
                                                          e.Idea.StageIdea != null &&
                                                          e.Idea.StageIdea.SemesterId == semesterId)
                                            .CountAsync();
        return number;
    }

    public async Task<Project?> GetProjectByLeaderId(Guid leaderId)
    {
        var project = await _context.Projects.Where(e => e.IsDeleted == false &&
                                                    e.LeaderId == leaderId &&
                                                    e.Status == ProjectStatus.Pending)
                                            .FirstOrDefaultAsync();
        return project;
    }

    public async Task<List<Project>> GetProjectBySemesterIdAndDefenseStage(Guid semesterId, int defenseStage)
    {
        var project = await _context.Projects.Where(e => e.IsDeleted == false &&
                                                    e.DefenseStage == defenseStage &&
                                                    e.Idea != null &&
                                                    e.Idea.StageIdea != null &&
                                                    e.Idea.StageIdea.Semester != null &&
                                                    e.Idea.StageIdea.Semester.Id == semesterId)
                                                .Include(e => e.Idea)
                                                .ToListAsync();
        return project;
    }

    public async Task<List<Project>?> GetInProgressProjectBySemesterId(Guid semesterId)
    {
        var projects = await _context.Projects.Where(e => e.Status == ProjectStatus.InProgress &&
                                                           e.IsDeleted == false &&
                                                           e.Idea != null &&
                                                           e.Idea.StageIdea != null &&
                                                           e.Idea.StageIdea.SemesterId == semesterId)
                                             .ToListAsync();
        return projects;
    }

    //public async Task<List<Project>?> GetProjectsInFourthWeekByToday()
    //{
    //    DateTime today = DateTime.UtcNow.Date;

    //    var projects = await _context.Projects.Where(p => p.Idea != null &&
    //                                        p.Idea.StageIdea != null &&
    //                                        p.Idea.StageIdea.Semester != null &&
    //                                        p.Idea.StageIdea.Semester.StartDate != null &&
    //                                        p.Idea.StageIdea.Semester.StartDate.Value.UtcDateTime.AddDays(3 * 7).Date == today)
    //                                        .Include(e => e.Idea).ThenInclude(e => e.StageIdea).ThenInclude(e => e.Semester)
    //                                        .ToListAsync();
    //    return projects;
    //}
}