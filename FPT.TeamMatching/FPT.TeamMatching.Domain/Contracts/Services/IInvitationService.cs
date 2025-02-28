﻿using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IInvitationService : IBaseService
{
    Task<BusinessResult> GetUserInvitationsByType(InvitationGetByTypeQuery query);
    Task<BusinessResult> CreateInvitationPending(InvitationCreatePendingCommand command);
}