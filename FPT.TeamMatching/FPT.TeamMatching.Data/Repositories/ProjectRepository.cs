﻿using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Utilities.Filters;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    private readonly FPTMatchingDbContext _context;
    private readonly ISemesterRepository _semesterRepository;

    public ProjectRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) : base(dbContext)
    {
        _context = dbContext;
        _semesterRepository = semesterRepository;
    }

    public async Task<Project?> GetProjectByUserIdLogin(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId &&
                                                               e.LeaveDate == null &&
                                                               (e.Status != TeamMemberStatus.Fail2) &&
                                                               e.IsDeleted == false)
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync();
        if (teamMember == null) return null;

        var queryable = GetQueryable().Where(e => e.Id == teamMember.ProjectId);

        queryable = queryable
            .Include(e => e.MentorTopicRequests)
            .Include(e => e.TeamMembers)
            .ThenInclude(e => e.User)
            .Include(e => e.Invitations)
            .Include(x => x.Reviews)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.StageIdea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty).ThenInclude(m => m.Profession);

        return queryable.FirstOrDefault();
    }
    
    
    public async Task<Project?> GetProjectByUserIdLoginFollowNewest(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId &&
                                                               e.LeaveDate == null &&
                                                               // (e.Status != TeamMemberStatus.Fail2) &&
                                                               e.IsDeleted == false)
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync();
        if (teamMember == null) return null;

        var queryable = GetQueryable().Where(e => e.Id == teamMember.ProjectId);

        queryable = queryable
            .Include(e => e.MentorTopicRequests)
            .Include(e => e.TeamMembers)
            .ThenInclude(e => e.User)
            .Include(e => e.Invitations)
            .Include(x => x.Reviews)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.StageIdea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty).ThenInclude(m => m.Profession);

        return queryable.FirstOrDefault();
    }
    
    public async Task<Project?> GetByIdForCheckMember(Guid? id)
    {
        var queryable = GetQueryable(x => x.Id == id);
        queryable = queryable.Include(m => m.TeamMembers)
            .Include(m => m.Topic)
            .ThenInclude(m => m.IdeaVersion);
        var entity = await queryable.FirstOrDefaultAsync();

        return entity;
    }

    public async Task<Project?> GetProjectInSemesterCurrentByUserIdLogin(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId &&
                                                               e.LeaveDate == null &&
                                                               (e.Status != TeamMemberStatus.Fail2) &&
                                                               e.IsDeleted == false)
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync();
        if (teamMember == null) return null;

        var semesterCurrent = await _semesterRepository.GetCurrentSemester();
        if (semesterCurrent == null) return null;

        var queryable = GetQueryable().Where(e => e.Id == teamMember.ProjectId);

        queryable = queryable.Include(e => e.TeamMembers)
            .ThenInclude(e => e.User)
            .Include(e => e.Invitations)
            .Include(x => x.Reviews)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.StageIdea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty).ThenInclude(m => m.Profession);

        queryable = queryable.Where(m => m.Topic != null &&
                                         m.Topic.IdeaVersion != null &&
                                         m.Topic.IdeaVersion.StageIdea != null &&
                                         m.Topic.IdeaVersion.StageIdea.SemesterId == semesterCurrent.Id
        );


        return queryable.FirstOrDefault();
    }
    
    
    public async Task<Project?> GetProjectInSemesterCurrentByUserIdLoginFollowNewest(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId &&
                                                               e.LeaveDate == null &&
                                                               // (e.Status != TeamMemberStatus.Fail2) &&
                                                               e.IsDeleted == false)
            .OrderByDescending(m => m.CreatedDate)
            .FirstOrDefaultAsync();
        if (teamMember == null) return null;

        var semesterCurrent = await _semesterRepository.GetCurrentSemester();
        if (semesterCurrent == null) return null;

        var queryable = GetQueryable().Where(e => e.Id == teamMember.ProjectId);

        queryable = queryable.Include(e => e.TeamMembers)
            .ThenInclude(e => e.User)
            .Include(e => e.Invitations)
            .Include(x => x.Reviews)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.StageIdea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty)
            .Include(x => x.Topic).ThenInclude(m => m.IdeaVersion).ThenInclude(m => m.Idea)
            .ThenInclude(m => m.Specialty).ThenInclude(m => m.Profession);

        queryable = queryable.Where(m => m.Topic != null &&
                                         m.Topic.IdeaVersion != null &&
                                         m.Topic.IdeaVersion.StageIdea != null &&
                                         m.Topic.IdeaVersion.StageIdea.SemesterId == semesterCurrent.Id
        );


        return queryable.FirstOrDefault();
    }

    public async Task<(List<Project>, int)> SearchProjects(ProjectSearchQuery query)
    {
        var queryable = GetQueryable();
        var semesterCurrent = await _semesterRepository.GetCurrentSemester();
        queryable = queryable.Include(e => e.TeamMembers)
            .ThenInclude(e => e.User)
            .ThenInclude(e => e.ProfileStudent)
            .ThenInclude(e => e.Specialty)
            .Include(e => e.Topic)
            .ThenInclude(e => e.IdeaVersion)
            .ThenInclude(e => e.Idea)
            .ThenInclude(e => e.Specialty)
            .ThenInclude(e => e.Profession)
            .Include(m => m.Topic)
            .ThenInclude(e => e.IdeaVersion)
            .ThenInclude(e => e.Idea)
            .ThenInclude(m => m.Owner)
            .Include(x => x.Reviews)
            .Include(x => x.Topic)
            .ThenInclude(x => x.IdeaVersion)
            .ThenInclude(x => x.Idea)
            .Include(x => x.MentorFeedback);

        if (query.IsHasTeam)
        {
            queryable = queryable.Where(m => m.TeamMembers.Count > 0 && m.TopicId != null);
        }

        if (query.SpecialtyId != null)
        {
            queryable = queryable.Where(m =>
                m.Topic != null && m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.Idea != null &&
                m.Topic.IdeaVersion.Idea.SpecialtyId == query.SpecialtyId);
        }

        if (query.ProfessionId != null)
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.Idea != null &&
                m.Topic.IdeaVersion.Idea.Specialty != null &&
                m.Topic.IdeaVersion.Idea.Specialty.ProfessionId == query.ProfessionId);
        }

        if (!string.IsNullOrEmpty(query.EnglishName))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.EnglishName != null &&
                m.Topic.IdeaVersion.EnglishName.ToLower().Trim()
                    .Contains(query.EnglishName.ToLower().Trim()));
        }

        queryable = semesterCurrent != null
            ? queryable.Where(m => m.Status != ProjectStatus.Canceled && m.Status != ProjectStatus.Pending)
            : queryable.Where(m => m.Status == ProjectStatus.Pending);


        queryable = BaseFilterHelper.Base(queryable, query);

        queryable = Sort(queryable, query);

        var total = queryable.Count();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    public async Task<List<Project>?> GetProjectsStartingNow()
    {
        var today = DateTime.UtcNow.AddHours(7).Date;
        var tomorrow = today.AddDays(1);
        var project = await _context.Projects
            .Include(e => e.Topic)
            .ThenInclude(e => e.IdeaVersion)
            .ThenInclude(e => e.Idea)
            .Where(p => p.IsDeleted == false &&
                        p.Status == ProjectStatus.InProgress &&
                        p.Topic != null &&
                        p.Topic.IdeaVersion != null &&
                        p.Topic.IdeaVersion.StageIdea != null &&
                        p.Topic.IdeaVersion.StageIdea.Semester != null &&
                        p.Topic.IdeaVersion.StageIdea.Semester.StartDate != null &&
                        p.Topic.IdeaVersion.StageIdea.Semester.StartDate.Value.AddHours(7) >= today &&
                        p.Topic.IdeaVersion.StageIdea.Semester.StartDate.Value.AddHours(7) < tomorrow
                // p.Topic.IdeaVersion.StageIdea.Semester.StartDate.Value.Day == DateTime.UtcNow.Day
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

    public async Task<int> NumberOfProjectInSemester(Guid semesterId)
    {
        var number = await _context.Projects.Where(e =>
                e.IsDeleted == false &&
                e.Leader != null &&
                e.Leader.UserXRoles.Any(m => m.SemesterId == semesterId))
            .CountAsync();
        return number;
    }

    public async Task<int> NumberOfInProgressProjectInSemester(Guid semesterId)
    {
        var number = await _context.Projects.Where(e => e.Status == ProjectStatus.InProgress &&
                                                        e.IsDeleted == false &&
                                                        e.Topic != null &&
                                                        e.Topic.IdeaVersion != null &&
                                                        e.Topic.IdeaVersion.StageIdea != null &&
                                                        e.Topic.IdeaVersion.StageIdea.SemesterId == semesterId)
            .CountAsync();
        return number;
    }

    public async Task<Project?> GetProjectByLeaderId(Guid? leaderId)
    {
        var project = await _context.Projects.Where(e => e.IsDeleted == false &&
                                                         e.LeaderId == leaderId &&
                                                         e.Status == ProjectStatus.Pending)
            .FirstOrDefaultAsync();
        return project;
    }

    public async Task<List<Project>> GetProjectBySemesterIdAndDefenseStage(Guid semesterId, int defenseStage)
    {
        var queryable = GetQueryable();
        var project = await queryable
            .Include(x => x.CapstoneSchedules)
            .Include(x => x.Topic)
            .ThenInclude(x => x.IdeaVersion)
            .ThenInclude(x => x.Idea)
            .Where(e => e.IsDeleted == false &&
                        e.DefenseStage == defenseStage &&
                        e.Topic != null &&
                        e.Topic.IdeaVersion != null &&
                        e.Topic.IdeaVersion.StageIdea != null &&
                        e.Topic.IdeaVersion.StageIdea.SemesterId == semesterId
            )
            .ToListAsync();
        return project;
    }

    public async Task<List<Project>?> GetInProgressProjectBySemesterId(Guid semesterId)
    {
        var projects = await GetQueryable().Where(e => e.Status == ProjectStatus.InProgress &&
                                                       e.IsDeleted == false &&
                                                       e.Topic != null &&
                                                       e.Topic.IdeaVersion != null &&
                                                       e.Topic.IdeaVersion.StageIdea != null &&
                                                       e.Topic.IdeaVersion.StageIdea.SemesterId == semesterId)
            .Include(x => x.Topic)
            .ThenInclude(x => x.IdeaVersion)
            .ThenInclude(x => x.Idea)
            .ThenInclude(x => x.Mentor)
            .Include(x => x.Topic)
            .ThenInclude(x => x.IdeaVersion)
            .ThenInclude(x => x.Idea)
            .ThenInclude(x => x.SubMentor)
            .ToListAsync();
        return projects;
    }

    public async Task<(List<Project>, int)> GetProjectsForMentor(ProjectGetListForMentorQuery query, Guid userId)
    {
        var queryable = GetQueryable();
        queryable = queryable
            .Include(x => x.Leader)
            .Include(m => m.Topic)
            .ThenInclude(x => x.IdeaVersion)
            .ThenInclude(x => x.Idea)
            .Include(m => m.Leader)
            .Include(m => m.MentorFeedback);

        if (query.Roles.Contains("Mentor") && query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.Idea != null &&
                (m.Topic.IdeaVersion.Idea.MentorId == userId ||
                 m.Topic.IdeaVersion.Idea.SubMentorId == userId));
        }
        else if (query.Roles.Contains("Mentor"))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.Idea != null &&
                m.Topic.IdeaVersion.Idea.MentorId == userId);
        }
        else if (query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.Idea != null &&
                m.Topic.IdeaVersion.Idea.SubMentorId == userId);
        }
        else
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.IdeaVersion != null &&
                m.Topic.IdeaVersion.Idea != null &&
                (m.Topic.IdeaVersion.Idea.MentorId == userId ||
                 m.Topic.IdeaVersion.Idea.SubMentorId == userId));
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        queryable = Sort(queryable, query);

        var total = queryable.Count();
        var results = query.IsPagination
            ? await GetQueryablePagination(queryable, query).ToListAsync()
            : await queryable.ToListAsync();

        return (results, query.IsPagination ? total : results.Count);
    }

    public async Task<Project?> GetProjectByTopicId(Guid topicId)
    {
        var queryable = GetQueryable();

        var project = await queryable.Where(e => e.IsDeleted == false &&
                                                 e.TopicId == topicId)
            .Include(e => e.Topic)
            .ThenInclude(e => e.IdeaVersion)
            .ThenInclude(e => e.Idea)
            .ThenInclude(e => e.Mentor)
            .FirstOrDefaultAsync();

        return project;
    }

    public async Task<List<Project>?> GetPendingProjectsWithNoTopicStartingBySemesterId(Guid semesterId)
    {
        var queryable = GetQueryable();

        var today = DateTime.UtcNow.AddHours(7).Date;
        var tomorrow = today.AddDays(1);
        var project = await queryable.Where(p =>
                p.IsDeleted == false &&
                p.TopicId == null &&
                p.Status == ProjectStatus.Pending &&
                // p.Leader != null &&
                p.Leader.UserXRoles.Any(r => r.SemesterId == semesterId &&
                                             r.Semester != null &&
                                             r.Semester.StartDate.Value.AddHours(7) >= today &&
                                             r.Semester.StartDate.Value.AddHours(7) < tomorrow))
            .ToListAsync();
        return project;
    }

    public async Task<List<Project>?> GetPendingProjectsWithTopicStartingBySemesterId(Guid semesterId)
    {
        var queryable = GetQueryable();

        var today = DateTime.UtcNow.AddHours(7).Date;
        var tomorrow = today.AddDays(1);

        var project = await queryable.Where(p => p.IsDeleted == false &&
                                                 p.Topic != null &&
                                                 p.Status == ProjectStatus.Pending &&
                                                 p.Topic.IdeaVersion != null &&
                                                 p.Topic.IdeaVersion.StageIdea != null &&
                                                 p.Topic.IdeaVersion.StageIdea.Semester != null &&
                                                 p.Topic.IdeaVersion.StageIdea.Semester.StartDate != null &&
                                                 p.Topic.IdeaVersion.StageIdea.Semester.StartDate.Value.AddHours(7) >=
                                                 today && p.Topic.IdeaVersion.StageIdea.Semester.StartDate.Value
                                                     .AddHours(7) < tomorrow
                // p.Topic.IdeaVersion.StageIdea.Semester.StartDate.Value.Day == today
            )
            .ToListAsync();
        return project;
    }

    public async Task<bool> IsExistedTeamCode(string teamCode)
    {
        var isExist = await _context.Projects.Where(e => e.IsDeleted == false &&
                                                         e.TeamCode == teamCode).AnyAsync();

        return isExist;
    }
}