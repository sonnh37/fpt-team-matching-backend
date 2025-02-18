using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    private readonly FPTMatchingDbContext _context;
    public ProjectRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        _context = dbContext;
    }

    public async Task<Project?> GetProjectByUserIdLogin(Guid userId)
    {
        var teamMember = await _context.TeamMembers.Where(e => e.UserId == userId && e.LeaveDate == null).FirstOrDefaultAsync();
        if (teamMember != null)
        {
            var project = await _context.Projects.Where(e => e.Id == teamMember.ProjectId)
                                                .Include(e => e.TeamMembers).Include(e => e.Idea)
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
}