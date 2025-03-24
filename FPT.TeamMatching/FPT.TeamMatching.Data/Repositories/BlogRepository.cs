using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class BlogRepository : BaseRepository<Blog>, IBlogRepository
{
    private readonly FPTMatchingDbContext _context;
    public BlogRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<Blog>?> GetBlogFindMemberInCurrentSemester(Guid id)
    {
        var blogs = await _context.Blogs.Where(e => e.IsDeleted == false &&
                                                e.Type == Domain.Enums.BlogType.Recruit &&
                                                e.Project != null &&
                                                e.Project.Idea != null &&
                                                e.Project.Idea.StageIdea != null &&
                                                e.Project.Idea.StageIdea.Semester != null &&
                                                e.Project.Idea.StageIdea.Semester.Id == id)
                                                .ToListAsync();
        return blogs;
    }
}