using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaRequestRepository : BaseRepository<IdeaRequest>, IIdeaRequestRepository
{
    public IdeaRequestRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }
}