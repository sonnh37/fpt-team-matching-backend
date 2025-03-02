using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Data.Repositories;

public class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
{
    public InvitationRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}