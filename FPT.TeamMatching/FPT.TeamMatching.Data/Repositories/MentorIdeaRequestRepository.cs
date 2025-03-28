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
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorIdeaRequest;

namespace FPT.TeamMatching.Data.Repositories
{
    public class MentorIdeaRequestRepository : BaseRepository<MentorIdeaRequest>, IMentorIdeaRequestRepository
    {
        private readonly IUserRepository _userRepository;

        public MentorIdeaRequestRepository(FPTMatchingDbContext dbContext, IUserRepository userRepository) :
            base(dbContext)
        {
            _userRepository = userRepository;
        }

        private async Task<User?> GetUserById(Guid id)
        {
            var queryable = GetQueryable<User>();
            queryable = queryable.Where(m => m.Id == id)
                .Include(e => e.Projects);
            var entity = await queryable.FirstOrDefaultAsync();

            return entity;
        }

        public async Task<(List<MentorIdeaRequest>, int)> GetUserMentorIdeaRequests(MentorIdeaRequestGetAllQuery query,
            Guid userId)
        {
            var queryable = GetQueryable();
            // Check three 
            var user = await GetUserById(userId);
            if (user == null) return (new List<MentorIdeaRequest>(), -1);

            var userProjectIds = user.Projects.Select(p => p.Id).ToList();
            queryable = queryable.Where(m =>
                    m.ProjectId != null && userProjectIds.Contains(m.ProjectId.Value))
                .Include(m => m.Project)
                .Include(m => m.Idea);

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
    }
}