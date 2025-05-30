﻿using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Utilities.Filters;
using MongoDB.Driver.Linq;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicOld;

namespace FPT.TeamMatching.Data.Repositories
{
    public class TopicOldRepository 
    {
        //private readonly FPTMatchingDbContext _context;
        //private readonly ISemesterRepository _semesterRepository;

        //public TopicOldRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) : base(dbContext)
        //{
        //    _context = dbContext;
        //    _semesterRepository = semesterRepository;
        //}

        //public bool IsExistedTopicCode(string topicCode)
        //{
        //    var isExist = _context.Topics.Where(e => e.TopicCode == topicCode).Any();
        //    return isExist;
        //}

        //public int NumberOfTopicBySemesterId(Guid semesterId)
        //{
        //    var numberOfTopic = _context.Topics
        //        .Count(e => e.IsDeleted == false &&
        //                    e.IdeaVersion != null &&
        //                    e.IdeaVersion.StageIdea != null &&
        //                    e.IdeaVersion.StageIdea.Semester != null &&
        //                    e.IdeaVersion.StageIdea.Semester.Id == semesterId);
        //    return numberOfTopic;
        //}

        //public async Task<(List<Topic>, int)> GetTopicsOfSupervisors(TopicOldGetListOfSupervisorsQuery query)
        //{
        //    var queryable = GetQueryable();
        //    queryable = queryable.Include(m => m.MentorTopicRequests)
        //        .Include(m => m.IdeaVersion)
        //        .ThenInclude(m => m.StageIdea)
        //        .ThenInclude(m => m.Semester)
        //        .Include(m => m.IdeaVersion)
        //        .ThenInclude(m => m.Idea)
        //        .ThenInclude(m => m.Specialty).ThenInclude(m => m.Profession)
        //        .Include(m => m.IdeaVersion)
        //        .ThenInclude(m => m.Idea)
        //        .ThenInclude(m => m.Mentor)
        //        .Include(m => m.Project);

        //    // Get current or upcoming semester
        //    var semester = await _semesterRepository.GetCurrentSemester() ??
        //                   await _semesterRepository.GetUpComingSemester();

        //    if (semester == null)
        //    {
        //        return (new List<Topic>(), 0);
        //    }

        //    // Apply common filtering logic
        //    var currentDate = DateTime.UtcNow;
        //    queryable = queryable.Where(mx =>
        //        mx.IdeaVersion != null &&
        //        mx.IdeaVersion.StageIdea != null &&
        //        mx.IdeaVersion.Idea != null &&
        //        mx.IdeaVersion.Idea.Type == IdeaType.Lecturer &&
        //        mx.IdeaVersion.StageIdea.SemesterId == semester.Id &&
        //        mx.IdeaVersion.StageIdea.Semester != null &&
        //        mx.IdeaVersion.StageIdea.Semester.PublicTopicDate != null &&
        //        mx.IdeaVersion.StageIdea.Semester.PublicTopicDate <= currentDate &&
        //        mx.MentorTopicRequests.All(x => x.Status != MentorTopicRequestStatus.Approved));

        //    if (!string.IsNullOrEmpty(query.TopicCode))
        //    {
        //        queryable = queryable.Where(m => m.TopicCode != null && m.TopicCode.Contains(query.TopicCode));
        //    }

        //    if (!string.IsNullOrEmpty(query.EnglishName))
        //    {
        //        queryable = queryable.Where(m => m.IdeaVersion != null &&
        //                                         m.IdeaVersion.EnglishName != null &&
        //                                         m.IdeaVersion.EnglishName.Contains(query.EnglishName));
        //    }

        //    if (query.IsExistedTeam.HasValue)
        //    {
        //        queryable = queryable.Where(m => query.IsExistedTeam.Value ? m.Project != null : m.Project == null);
        //    }

        //    // Apply base filtering and sorting
        //    queryable = BaseFilterHelper.Base(queryable, query);
        //    queryable = Sort(queryable, query);

        //    // Handle pagination
        //    var total = await queryable.CountAsync();
        //    var results = query.IsPagination
        //        ? await GetQueryablePagination(queryable, query).ToListAsync()
        //        : await queryable.ToListAsync();

        //    return (results, query.IsPagination ? total : results.Count);
        //}

        //public async Task<Topic> GetByTopicId(Guid topicId)
        //{
        //    var result = await _context.Topics
        //        .Include(x => x.IdeaVersion)
        //        .ThenInclude(x => x.Idea)
        //        .Include(x => x.Project)
        //        .FirstOrDefaultAsync(x => x.Id == topicId);
        //    return result;
        //}

        public async Task<List<Topic>> GetAllTopicsByTopicCode(string[] topicCodes)
        {
            //var result = await _context.Topics
            //    .Include(x => x.Project)
            //    .Where(x => topicCodes.Contains(x.TopicCode))
            //    .ToListAsync();
            //return result;
            return null;
        }

        
    }
}