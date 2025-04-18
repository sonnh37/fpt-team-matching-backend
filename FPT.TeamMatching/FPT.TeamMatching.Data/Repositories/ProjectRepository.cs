﻿using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Utilities.Filters;
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
                                                               //(e.Status != TeamMemberStatus.Failed ||
                                                               // e.Status != TeamMemberStatus.Passed) &&
                                                                 (e.Status == TeamMemberStatus.InProgress ||
                                                                e.Status == TeamMemberStatus.Pending) &&
                                                               e.IsDeleted == false)
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync();
        if (teamMember != null)
        {
            var project = await _context.Projects.Where(e => e.Id == teamMember.ProjectId)
                .Include(e => e.TeamMembers)
                //sua db
                //.ThenInclude(e => e.User).Include(e => e.Idea)
                //.ThenInclude(e => e.Specialty)
                //.ThenInclude(e => e.Profession)
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
            .Where(p => p.IsDeleted == false 
                        //sua db
                        //&&
                        //p.Idea != null &&
                        //p.Idea.StageIdea != null &&
                        //p.Idea.StageIdea.Semester != null &&
                        //p.Idea.StageIdea.Semester != null &&
                        //p.Idea.StageIdea.Semester.StartDate != null &&
                        //p.Idea.StageIdea.Semester.StartDate.Value.UtcDateTime.Date == today
                        )
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
                                                        e.IsDeleted == false
                                                        //sua db
                                                        //&&
                                                        //e.Idea != null &&
                                                        //e.Idea.StageIdea != null &&
                                                        //e.Idea.StageIdea.SemesterId == semesterId
                                                        )
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
                                                         e.DefenseStage == defenseStage
                                                         //sua db
                                                         //&&
                                                         //e.Idea != null &&
                                                         //e.Idea.StageIdea != null &&
                                                         //e.Idea.StageIdea.Semester != null &&
                                                         //e.Idea.StageIdea.Semester.Id == semesterId
                                                         )
            //sua db
            //.Include(e => e.Idea)
            .ToListAsync();
        return project;
    }

    public async Task<List<Project>?> GetInProgressProjectBySemesterId(Guid semesterId)
    {
        var projects = await _context.Projects.Where(e => e.Status == ProjectStatus.InProgress &&
                                                          e.IsDeleted == false 
                                                          //sua db
                                                          //&&
                                                          //e.Idea != null &&
                                                          //e.Idea.StageIdea != null &&
                                                          //e.Idea.StageIdea.SemesterId == semesterId
                                                          )
            //sua db
            //.Include(e => e.Idea).ThenInclude(e => e.Mentor)
            //.Include(e => e.Idea).ThenInclude(e => e.SubMentor)
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

    public async Task<(List<Project>, int)> GetProjectsForMentor(ProjectGetListForMentorQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable
            //sua db
            //.Include(m => m.Idea)
            .Include(m => m.Leader)
            .Include(m => m.MentorFeedback);

        //sua db
        //queryable = queryable.Where(m => m.Idea != null && m.Idea.MentorId == userId);
        queryable = queryable.Where(m => m.Topic.IdeaVersionId != null && m.Topic.IdeaVersionId == userId);

        queryable = BaseFilterHelper.Base(queryable, query);
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
}