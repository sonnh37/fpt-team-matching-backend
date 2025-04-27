using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Utilities.Filters;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories
{
    public class TopicRepository : BaseRepository<Topic>, ITopicRepository
    {
        private readonly FPTMatchingDbContext _context;
        public TopicRepository(FPTMatchingDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public bool IsExistedTopicCode(string topicCode)
        {
            var isExist = _context.Topics.Where(e => e.TopicCode == topicCode).Any();
            return isExist;
        }

        public int NumberOfTopicBySemesterId(Guid semesterId)
        {
            var numberOfTopic = _context.Topics.Where(e => e.IsDeleted == false &&
                                                    e.IdeaVersion != null &&
                                                    e.IdeaVersion.StageIdea != null &&
                                                    e.IdeaVersion.StageIdea.Semester != null &&
                                                    e.IdeaVersion.StageIdea.Semester.Id == semesterId)
                                                .Count();
            return numberOfTopic;
        }
        
        public async Task<List<Topic>> GetAllTopicsByTopicCode(string[] topicCodes)
        {
            var result = await _context.Topics
                .Include(x => x.Project)
                .Where(x => topicCodes.Contains(x.TopicCode))
                .ToListAsync();
            return result;
        }
        
        public async Task<(List<Topic>, int)> GetTopicsForMentor(TopicGetListForMentorQuery query, Guid userId)
        {
            var queryable = GetQueryable();
            queryable = queryable
                .Include(x => x.IdeaVersion).ThenInclude(m => m.Idea)
                .Include(x => x.IdeaVersion).ThenInclude(m => m.IdeaVersionRequests)
                .Include(m => m.Project)
                .Include(m => m.MentorTopicRequests)
                .Include(m => m.TopicVersions);

            queryable = queryable.Where(m => 
                                             m.IdeaVersion != null &&
                                             m.IdeaVersion.Idea != null &&
                                             m.IdeaVersion.Idea.MentorId != null &&
                                             m.IdeaVersion.Idea.MentorId == userId);

            queryable = BaseFilterHelper.Base(queryable, query);
        
            queryable = Sort(queryable, query);
    
            var total = queryable.Count();
            var results = query.IsPagination 
                ? await GetQueryablePagination(queryable, query).ToListAsync()
                : await queryable.ToListAsync();

            return (results, query.IsPagination ? total : results.Count);
        }

        public async Task<List<Topic>> GetTopicByIdeaVersionId(List<Guid?> ideaVersionIds)
        {
            var queryable = GetQueryable(x => ideaVersionIds.Contains(x.IdeaVersionId));
            var entities = await queryable.ToListAsync();

            return entities;
        }
    }
}
