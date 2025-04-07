using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class InvitationService : BaseService<Invitation>, IInvitationService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly INotificationService _notificationService;

    public InvitationService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
    {
        _invitationRepository = unitOfWork.InvitationRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _notificationService = notificationService;
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

    public async Task<BusinessResult> GetUserInvitationsByStatus(InvitationGetListForUserByStatus query)
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
            var (data, total) = await _invitationRepository.GetUserInvitationsByStatus(query, userId);

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
    
    public async Task<BusinessResult> GetLeaderInvitationsByType(InvitationGetByTypeQuery query)
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
            var userInTeamMember = await _teamMemberRepository.GetTeamMemberActiveByUserId(userId);
            if (userInTeamMember == null) return HandlerFail("You not in team member");
            var isLeader = userInTeamMember.Role == TeamMemberRole.Leader;
            if(!isLeader) return HandlerFail("You do not have role leader team member");
            
            // get by type
            var (data, total) = await _invitationRepository.GetLeaderInvitationsByType(query, userId);

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

    public async Task<BusinessResult> CreatePendingByStudent(InvitationStudentCreatePendingCommand command)
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

            var haveInvite =
                await _invitationRepository.GetInvitationOfUserByProjectId((Guid)command.ProjectId, user.Id);
            if (haveInvite != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Student has request join team");
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
                //noti cho leader 
                var noti = new NotificationCreateForIndividual
                {
                    UserId = project.LeaderId,
                    Description = user.Code + " đã gửi yêu cầu tham gia nhóm",
                };
                await _notificationService.CreateForUser(noti);

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

    public async Task<BusinessResult> CreatePendingByTeam(InvitationTeamCreatePendingCommand command)
    {
        try
        {
            //check sl trong team
            bool members = await CheckCountMembersInTeam((Guid)command.ProjectId);
            if (!members)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Project has maximum members");
            }

            //check student co idea pending hay approve k
            var haveIdea = await StudentHaveIdea((Guid)command.ReceiverId);
            if (haveIdea)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Student has idea");
            }

            //check student trong teammember in process OR pass
            var inTeamMember = await StudentInTeamMember((Guid)command.ReceiverId);
            if (inTeamMember)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Student is in team now");
            }

            bool isSucess = await TeamCreateAsync(command);
            if (isSucess)
            {
                //noti lời mời vào nhóm
                var project = await _projectRepository.GetById((Guid)command.ProjectId);
                if (project == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy project");
                }
                var teamName = "";
                if (project.TeamName != null)
                {
                    teamName = project.TeamName;
                }
                var noti = new NotificationCreateForIndividual
                {
                    UserId = command.ReceiverId,
                    Description = "Nhóm " + teamName + " đã gửi lời mời tham gia nhóm",
                };
                await _notificationService.CreateForUser(noti);
                //
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

    // phản hồi lời mời từ team bởi cá nhân
    public async Task<BusinessResult> ApproveOrRejectInvitationFromTeamByMe(InvitationUpdateCommand command)
    {
        try
        {
            var userIdFromClaim = GetUserIdFromClaims();
            if (userIdFromClaim == null) return HandlerFailAuth();

            var userId = userIdFromClaim.Value;
            
            var invitation = await _invitationRepository.GetById(command.Id, true);
            if (invitation == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Team haven't sent invitation!");
            }
            
            if (command.Status == InvitationStatus.Rejected)
            {
                _invitationRepository.DeletePermanently(invitation);
                var saveChange_ = await _unitOfWork.SaveChanges();
                if (!saveChange_)
                {
                    return HandlerFail("Can not saving changes!");
                }
                //noti từ chối lời mời vào nhóm
                var noti = new NotificationCreateForIndividual
                {
                    UserId = invitation.SenderId,
                    Description = invitation.Receiver.Code + "đã từ chối lời mời tham gia nhóm của bạn",
                };
                await _notificationService.CreateForUser(noti);
                //
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }

            var teamMember = new TeamMember
            {
                UserId = userId,
                ProjectId = invitation.ProjectId,
                Role = TeamMemberRole.Member,
                JoinDate = DateTime.UtcNow,
                LeaveDate = null,
                Status = TeamMemberStatus.Pending
            };
            await SetBaseEntityForCreation(teamMember);
            _teamMemberRepository.Add(teamMember);
            var saveChange = await _unitOfWork.SaveChanges();
            if (saveChange)
            {
                invitation.Status = InvitationStatus.Accepted;
                await SetBaseEntityForUpdate(invitation);
                _invitationRepository.Update(invitation);
                var saveChange_ = await _unitOfWork.SaveChanges();
                if (saveChange_)
                {
                    //noti đồng ý lời mời vào nhóm
                    var noti = new NotificationCreateForTeam
                    {
                        ProjectId = invitation.ProjectId,
                        Description = invitation.Receiver.Code + "đã đồng ý lời mời tham gia nhóm của bạn",
                    };
                    await _notificationService.CreateForTeam(noti);
                    //
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                
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

    // phản hồi lời mời từ cá nhân bởi leader
    public async Task<BusinessResult> ApproveOrRejectInvitationFromPersonalizeByLeader(InvitationUpdateCommand command)
    {
        try
        {
            var invitation = await _invitationRepository.GetById(command.Id, true);
            if (invitation == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Personal haven't sent invitation for Team!");
            }

            if (command.Status == InvitationStatus.Rejected)
            {
                _invitationRepository.DeletePermanently(invitation);
                //noti phản hồi từ chối của team đến receiver
                var teamName = "";
                if (invitation.Project?.TeamName != null)
                {
                    teamName = invitation.Project.TeamName;
                }
                var noti = new NotificationCreateForIndividual
                {
                    UserId = invitation.SenderId,
                    Description = "Lời mời tham gia nhóm " + teamName + " của bạn đã bị từ chối",
                };
                await _notificationService.CreateForUser(noti);
                //
                var saveChange_ = await _unitOfWork.SaveChanges();
                if (!saveChange_)
                {
                    return HandlerFail("Can not saving changes!");
                }
                
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }

            var teamMember = new TeamMember
            {
                UserId = invitation.SenderId,
                ProjectId = invitation.ProjectId,
                Role = TeamMemberRole.Member,
                JoinDate = DateTime.UtcNow,
                LeaveDate = null,
                Status = TeamMemberStatus.Pending
            };
            await SetBaseEntityForCreation(teamMember);
            _teamMemberRepository.Add(teamMember);
            var saveChange = await _unitOfWork.SaveChanges();
            if (saveChange)
            {
                invitation.Status = InvitationStatus.Accepted;
                await SetBaseEntityForUpdate(invitation);
                _invitationRepository.Update(invitation);
                var saveChange_ = await _unitOfWork.SaveChanges();
                if (saveChange_)
                {
                    //noti phản hồi đồng ý của team đến receiver
                    ////noti đến team
                    var noti1 = new NotificationCreateForTeam
                    {
                        ProjectId = invitation.Project?.Id,
                        Description = invitation.Receiver?.Code + " đã được chấp nhận tham gia nhóm của bạn",
                    };  
                    await _notificationService.CreateForTeam(noti1);


                    ////noti đến sender
                    var teamName = "";
                    if (invitation.Project.TeamName != null)
                    {
                        teamName = invitation.Project.TeamName;
                    }
                    var noti2 = new NotificationCreateForIndividual
                    {
                        UserId = invitation.SenderId,
                        Description = "Lời mời tham gia nhóm " + teamName + " của bạn đã được chấp nhận",
                    };
                    await _notificationService.CreateForUser(noti2);
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

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

    //public async Task<BusinessResult> ApproveInvitationOfTeamByMe(Guid projectId)
    //{
    //    try
    //    {
    //        var user = await GetUserAsync();
    //        var invitation = await _invitationRepository.GetInvitationOfTeamByProjectIdAndMe(projectId, user.Id);
    //        if (invitation == null)
    //        {
    //            return new ResponseBuilder()
    //                .WithStatus(Const.FAIL_CODE)
    //                .WithMessage("Team haven't sent invitation!");
    //        }
    //        //add teammember
    //        var teamMember = new TeamMember
    //        {
    //            UserId = user.Id,
    //            ProjectId = invitation.ProjectId,
    //            Role = TeamMemberRole.Member,
    //            JoinDate = DateTime.UtcNow,
    //            LeaveDate = null,
    //            Status = TeamMemberStatus.Pending
    //        };
    //        await SetBaseEntityForCreation(teamMember);
    //        _teamMemberRepository.Add(teamMember);
    //        var saveChange = await _unitOfWork.SaveChanges();
    //        if (saveChange)
    //        {
    //            //update status invitation
    //            invitation.Status = InvitationStatus.Accepted;
    //            await SetBaseEntityForUpdate(invitation);
    //            _invitationRepository.Update(invitation);
    //            var saveChange_ = await _unitOfWork.SaveChanges();
    //            if (saveChange_)
    //            {
    //                //update +1 teamszie
    //                var project = await _projectRepository.GetById(projectId);
    //                project.TeamSize += 1;
    //                await SetBaseEntityForUpdate(project);
    //                _projectRepository.Update(project);
    //                var saveChange__ = await _unitOfWork.SaveChanges();
    //                if (saveChange__)
    //                {
    //                    return new ResponseBuilder()
    //                        .WithStatus(Const.SUCCESS_CODE)
    //                        .WithMessage(Const.SUCCESS_DELETE_MSG);
    //                }
    //                return new ResponseBuilder()
    //                .WithStatus(Const.FAIL_CODE)
    //                .WithMessage(Const.FAIL_DELETE_MSG);
    //            }

    //            return new ResponseBuilder()
    //                .WithStatus(Const.FAIL_CODE)
    //                .WithMessage(Const.FAIL_DELETE_MSG);
    //        }

    //        return new ResponseBuilder()
    //            .WithStatus(Const.FAIL_CODE)
    //            .WithMessage(Const.FAIL_DELETE_MSG);
    //    }
    //    catch (Exception ex)
    //    {
    //        var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
    //        return new ResponseBuilder()
    //            .WithStatus(Const.FAIL_CODE)
    //            .WithMessage(errorMessage);
    //    }
    //}

    //public async Task<BusinessResult> CancelInvitationOfTeamByMe(Guid projectId)
    //{
    //    try
    //    {
    //        var user = await GetUserAsync();
    //        var invitation = await _invitationRepository.GetInvitationOfTeamByProjectIdAndMe(projectId, user.Id);
    //        if (invitation == null)
    //        {
    //            return new ResponseBuilder()
    //                .WithStatus(Const.FAIL_CODE)
    //                .WithMessage("Team haven't sent invitation!");
    //        }

    //        _invitationRepository.DeletePermanently(invitation);
    //        var saveChange_ = await _unitOfWork.SaveChanges();
    //        if (saveChange_)
    //        {
    //            return new ResponseBuilder()
    //                .WithStatus(Const.SUCCESS_CODE)
    //                .WithMessage(Const.SUCCESS_DELETE_MSG);
    //        }

    //        return new ResponseBuilder()
    //            .WithStatus(Const.FAIL_CODE)
    //            .WithMessage(Const.FAIL_DELETE_MSG);
    //    }
    //    catch (Exception ex)
    //    {
    //        var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
    //        return new ResponseBuilder()
    //            .WithStatus(Const.FAIL_CODE)
    //            .WithMessage(errorMessage);
    //    }
    //}

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
        var ideas = await _ideaRepository.GetIdeasByUserId(userId);
        bool haveIdea = false;
        if (!ideas.Any())
        {
            return haveIdea = false;
        }

        foreach (var idea in ideas)
        {
            if (idea.Status != IdeaStatus.Rejected)
            {
                haveIdea = true;
            }
        }

        return haveIdea;
    }

    private async Task<bool> StudentInTeamMember(Guid userId)
    {
        var teamMembers = await _teamMemberRepository.GetTeamMemberByUserId(userId);
        bool haveTeamMember = false;
        if (!teamMembers.Any())
        {
            haveTeamMember = false;
        }

        foreach (var teamMember in teamMembers)
        {
            if (teamMember.Status != TeamMemberStatus.Failed)
            {
                haveTeamMember = true;
            }
        }

        return haveTeamMember;
    }

    private async Task<bool> CheckCountMembersInTeam(Guid projectId)
    {
        var teammember = await _projectRepository.GetById(projectId);
        var count = teammember.TeamMembers.Where(x => x.IsDeleted = false).Count();
        if (count <= teammember.TeamSize)
        {
            return true;
        }

        return false;
    }
}