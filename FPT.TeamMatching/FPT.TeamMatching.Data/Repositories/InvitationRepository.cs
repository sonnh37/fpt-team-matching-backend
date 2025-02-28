using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    public InvitationRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }

    public async Task<Invitation?> GetInvitationOfUserByProjectId(Guid projectId, Guid userId)
    {
        var i = await _dbContext.Invitations.Where(e => e.Status != null 
                                            && e.ProjectId == projectId 
                                            && e.SenderId == userId 
                                            && e.Status.Value == InvitationStatus.Pending)
                                            .SingleOrDefaultAsync();
        return i;
    }
}