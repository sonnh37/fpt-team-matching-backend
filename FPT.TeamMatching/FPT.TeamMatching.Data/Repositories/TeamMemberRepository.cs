using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Data.Repositories;

public class TeamMemberRepository : BaseRepository<TeamMember>, ITeamMemberRepository
{
    public TeamMemberRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }
}