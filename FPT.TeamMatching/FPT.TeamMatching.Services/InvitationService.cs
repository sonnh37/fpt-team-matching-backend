using AutoMapper;
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
    private readonly IIdeaRepository _ideaRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public InvitationService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _invitationRepository = unitOfWork.InvitationRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
    }

    public async Task<BusinessResult> CheckIfStudentSendInvitationByProjectId(Guid projectId)
    {
        try
        {
            bool hasSent = true;
            var user = await GetUserAsync();
            if (user == null) return HandlerFail("Not logged in");
            var i = await _invitationRepository.GetInvitationOfUserByProjectId(projectId, user.Id);
            if (i == null)
            {
                hasSent = false;
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, hasSent);
            }

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, hasSent);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetUserInvitationsByType(InvitationGetByTypeQuery query)
    {
        try
        {
            List<InvitationResult>? results;
            var userIdClaim = GetUserIdFromClaims();

            if (userIdClaim == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("You need to authenticate with TeamMatching.");

            var userId = userIdClaim.Value;
            // get by type
            var (data, total) = await _invitationRepository.GetUserInvitationsByType(query, userId);

            results = _mapper.Map<List<InvitationResult>>(data);

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);

            // GetAll with pagination
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                .WithData(tableResponse)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> StudentCreatePending(InvitationStudentCreatePendingCommand command)
    {
        try
        {
            var invitation = _mapper.Map<Invitation>(command);
            var user = await GetUserAsync();
            if (user == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("User do not exist");
            }
            //check student co idea pending hay approve k
            var haveIdea = await StudentHaveIdea(user.Id);
            if (haveIdea)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Student has idea");
            }
            //check student trong teammember in process OR pass
            var inTeamMember = await StudentInTeamMember(user.Id);
            if (inTeamMember)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Student is in team now");
            }
            //check project exist
            var project = await _projectRepository.GetById((Guid)command.ProjectId);
            if (project == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Project do not exist");
            }

            invitation.Status = InvitationStatus.Pending;
            invitation.SenderId = user.Id;
            invitation.ReceiverId = project.LeaderId;
            invitation.Type = InvitationType.SentByStudent;
            await SetBaseEntityForCreation(invitation);
            _invitationRepository.Add(invitation);
            bool saveChange = await _unitOfWork.SaveChanges();
            if (saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> TeamCreatePending(InvitationTeamCreatePendingCommand command)
    {
        try
        {
            bool isSucess = await TeamCreateAsync(command);
            if (isSucess)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> DeletePermanentInvitation(Guid projectId)
    {
        try
        {
            var user = await GetUserAsync();
            if (user == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("User haven't login!");
            }
            var invitation = await _invitationRepository.GetInvitationOfUserByProjectId(projectId, user.Id);
            if (invitation == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("User haven't sent invitation to given project!");
            }
            _invitationRepository.DeletePermanently(invitation);
            bool saveChange = await _unitOfWork.SaveChanges();
            if (saveChange)
            {
                return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_DELETE_MSG);
            }
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_DELETE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    private async Task<bool> TeamCreateAsync(InvitationTeamCreatePendingCommand command)
    {
        var invitation = _mapper.Map<Invitation>(command);
        var user = await GetUserAsync();
        if (user == null) return false;
        if (command.ProjectId == null) return false;
        var project = await _projectRepository.GetById(command.ProjectId.Value);
        if (project != null)
        {
            invitation.Status = InvitationStatus.Pending;
            invitation.SenderId = user.Id;
            invitation.Type = InvitationType.SendByTeam;
            await SetBaseEntityForCreation(invitation);
            _invitationRepository.Add(invitation);
            bool saveChange = await _unitOfWork.SaveChanges();
            return saveChange;
        }

        return false;
    }

    private async Task<bool> StudentHaveIdea(Guid userId)
    {
        var ideas = _ideaRepository.GetIdeasByUserId(userId);
        bool haveIdea = true;
        if (ideas == null)
        {
            haveIdea = false;
        }
        foreach (var idea in await ideas)
        {
            if (idea.Status == IdeaStatus.Rejected)
            {
                haveIdea = false;
            }
        }
        return haveIdea;
    }

    private async Task<bool> StudentInTeamMember(Guid userId)
    {
        var teamMembers = _teamMemberRepository.GetTeamMemberByUserId(userId);
        bool haveTeamMember = true;
        if (teamMembers == null)
        {
            haveTeamMember = false;
        }
        foreach (var teamMember in await teamMembers)
        {
            if (teamMember.Status == TeamMemberStatus.Failed)
            {
                haveTeamMember = false;
            }
        }
        return haveTeamMember;
    }


}