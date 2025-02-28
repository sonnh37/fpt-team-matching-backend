﻿using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Utilities.Filters;
using FPT.TeamMatching.Services.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FPT.TeamMatching.Services;

public class InvitationService : BaseService<Invitation>, IInvitationService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IProjectRepository _projectRepository;

    public InvitationService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _invitationRepository = unitOfWork.InvitationRepository;
        _projectRepository = unitOfWork.ProjectRepository;
    }


    public async Task<BusinessResult> GetUserInvitationsByType(InvitationGetByTypeQuery query)
    {
        try
        {
            List<InvitationResult>? results;
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("You need to authenticate with TeamMatching.")
                    .Build();
            var userId = Guid.Parse(userIdClaim);
            // get by type
            var queryable = _invitationRepository.GetQueryable(m => m.Type == query.Type);
            queryable = IncludeHelper.Apply(queryable);
            // type: joinRequest 
            // => verify userId with receiver
            // type: requestToJoin 
            // => verify userId with sender

            queryable = query.Type switch
            {
                InvitationType.SentByStudent => queryable.Where(m => m.SenderId == userId),
                InvitationType.SendByTeam => queryable.Where(m => m.ReceiverId == userId),
                _ => queryable
            };

            if (!query.IsPagination)
            {
                var allData = await queryable.ToListAsync();
                results = _mapper.Map<List<InvitationResult>>(allData);
                if (!results.Any())
                    return new ResponseBuilder<InvitationResult>()
                        .WithData(results)
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG)
                        .Build();

                return new ResponseBuilder<InvitationResult>()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG)
                    .Build();
            }

            var totalOrigin = queryable.Count();
            var result = await _invitationRepository.ApplySortingAndPaging(queryable, query);
            // create results table response
            results = _mapper.Map<List<InvitationResult>>(result);
            var tableResponse = new ResultsTableResponse<InvitationResult>
            {
                GetQueryableQuery = query,
                Item = (results, totalOrigin)
            };

            if (!results.Any())
                return new ResponseBuilder<InvitationResult>()
                    .WithData(tableResponse)
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG)
                    .Build();


            return new ResponseBuilder<InvitationResult>()
                .WithData(tableResponse)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG)
                .Build();
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }

    public async Task<BusinessResult> StudentCreatePending(InvitationStudentCreatePendingCommand command)
    {
        try
        {
            bool isSucess = await StudentCreateAsync(command);
            if (isSucess)
            {
                return new ResponseBuilder<Invitation>()
                                .WithStatus(Const.SUCCESS_CODE)
                                .WithMessage(Const.SUCCESS_SAVE_MSG)
                                .Build();
            }
            return new ResponseBuilder<Invitation>()
                                .WithStatus(Const.FAIL_CODE)
                                .WithMessage(Const.FAIL_SAVE_MSG)
                                .Build();
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }

    public async Task<BusinessResult> TeamCreatePending(InvitationTeamCreatePendingCommand command)
    {
        try
        {
            bool isSucess = await TeamCreateAsync(command);
            if (isSucess)
            {
                return new ResponseBuilder<Invitation>()
                                .WithStatus(Const.SUCCESS_CODE)
                                .WithMessage(Const.SUCCESS_SAVE_MSG)
                                .Build();
            }
            return new ResponseBuilder<Invitation>()
                                .WithStatus(Const.FAIL_CODE)
                                .WithMessage(Const.FAIL_SAVE_MSG)
                                .Build();
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }

    private async Task<bool> StudentCreateAsync(InvitationStudentCreatePendingCommand command)
    {
        var invitation = _mapper.Map<Invitation>(command);
        var user = GetUser();
        var project = await _projectRepository.GetById((Guid)command.ProjectId);
        if (project != null && user != null)
        {
            invitation.Status = InvitationStatus.Pending;
            invitation.SenderId = user.Id;
            invitation.ReceiverId = project.LeaderId;
            invitation.Type = InvitationType.SentByStudent;
            InitializeBaseEntityForCreate(invitation);
            _invitationRepository.Add(invitation);
            bool saveChange = await _unitOfWork.SaveChanges();
            return saveChange;
        }
        return false;
    }

    private async Task<bool> TeamCreateAsync(InvitationTeamCreatePendingCommand command)
    {
        var invitation = _mapper.Map<Invitation>(command);
        var user = GetUser();
        var project = await _projectRepository.GetById((Guid)command.ProjectId);
        if (project != null && user != null)
        {
            invitation.Status = InvitationStatus.Pending;
            invitation.SenderId = user.Id;
            invitation.Type = InvitationType.SendByTeam;
            InitializeBaseEntityForCreate(invitation);
            _invitationRepository.Add(invitation);
            bool saveChange = await _unitOfWork.SaveChanges();
            return saveChange;
        }
        return false;
    }
}