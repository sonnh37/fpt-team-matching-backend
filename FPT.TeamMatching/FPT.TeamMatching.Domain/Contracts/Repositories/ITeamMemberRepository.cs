﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface ITeamMemberRepository : IBaseRepository<TeamMember>
{
    Task<TeamMember?> GetTeamMemberActiveByUserId(Guid userId);

    Task<List<TeamMember>> GetTeamMemberByUserId(Guid userId);
    Task<TeamMember> GetMemberByUserId(Guid userId);
}