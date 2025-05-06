using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
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
    private readonly IUserRepository _userRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IStageIdeaRepositoty _stageIdeaRepositoty;
    private readonly INotificationService _notificationService;

    public InvitationService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(
        mapper, unitOfWork)
    {
        _invitationRepository = unitOfWork.InvitationRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _userRepository = unitOfWork.UserRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _stageIdeaRepositoty = unitOfWork.StageIdeaRepository;
        _notificationService = notificationService;
    }

    public async Task<BusinessResult> CheckIfStudentSendInvitationByProjectId(Guid projectId)
    {
        try
        {
            bool hasSent = true;
            var user = await GetUserAsync();
            if (user == null) return HandlerFail("Bạn chưa đăng nhập");
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
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null) return HandlerFailAuth();
            var userId = userIdClaim.Value;
            // get by type
            var (data, total) = await _invitationRepository.GetUserInvitationsByStatus(query, userId);
            var results = _mapper.Map<List<InvitationResult>>(data);

            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
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
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null)
                return HandlerFailAuth();

            var userId = userIdClaim.Value;
            // get by type
            var (data, total) = await _invitationRepository.GetUserInvitationsByType(query, userId);
            var results = _mapper.Map<List<InvitationResult>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
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
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null)
                return HandlerFailAuth();

            var userId = userIdClaim.Value;
            var userInTeamMember = await _teamMemberRepository.GetTeamMemberActiveByUserId(userId);
            if (userInTeamMember == null) return HandlerFail("Bạn chưa có nhóm");

            var isLeader = userInTeamMember.Role == TeamMemberRole.Leader;
            if (!isLeader) return HandlerFail("Bạn không phải là trưởng nhóm");
            // get by type
            var (data, total) = await _invitationRepository.GetLeaderInvitationsByType(query, userId);
            var results = _mapper.Map<List<InvitationResult>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
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
                    .WithMessage("Không tìm thấy người dùng");
            }

            var haveInvite =
                await _invitationRepository.GetInvitationOfUserByProjectId((Guid)command.ProjectId, user.Id);
            if (haveInvite != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Sinh viên đã gửi yêu cầu tham gia nhóm");
            }

            //check student co idea pending hay approve k
            var semesterUpComing = await _semesterRepository.GetUpComingSemester();
            if (semesterUpComing == null)
                return HandlerFail("Hệ thống chưa cập nhật.");
            var idea = await _ideaRepository.GetIdeaNotRejectOfUserInSemester(user.Id, semesterUpComing.Id);
            if (idea != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Người dùng đã nộp ý tưởng");
            }

            //check student trong teammember in process OR pass
            var inTeamMember = await StudentInTeamMember(user.Id);
            if (inTeamMember)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Người dùng đã có nhóm");
            }

            //check project exist
            var project = await _projectRepository.GetById((Guid)command.ProjectId, true);
            if (project == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy dự án");
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
            //lay ra stageIdea hien tai
            var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
            if (stageIdea == null) return HandlerFail("Không có đợt duyệt ứng với ngày hiện tại!");

            //ki cua stage idea
            var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
            if (semester == null) return HandlerFail("Không có kì ứng với đợt duyệt hiện tại!");

            //check nguoi nhan
            var user = await GetUserAsync();
            if (user == null) return HandlerFailAuth();
            //check người nhận là student
            var isStudent = await _userRepository.CheckRoleOfUserInSemester(command.ReceiverId, "Student",semester.Id);
            if (!isStudent)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Người nhận không phải là sinh viên");
            }
            
            var project = await _projectRepository.GetByIdForCheckMember(command.ProjectId);
            if (project == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy nhóm");
            }
            //check sl trong team
            var teamSizeCurrent = project.TeamMembers.Count(x => x.IsDeleted = false);
            var teamSizeRequired = project.Topic != null ? project.Topic?.IdeaVersion?.TeamSize : 5;
            if (teamSizeCurrent >= teamSizeRequired)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhóm đã đủ thành viên");
            }

            //check student co idea pending hay approve k
            var semesterUpComing = await _semesterRepository.GetUpComingSemester();
            if (semesterUpComing == null)
                return HandlerFail("Hệ thống chưa cập nhật.");
            var idea = await _ideaRepository.GetIdeaNotRejectOfUserInSemester(command.ReceiverId.Value, semesterUpComing.Id);
            if (idea != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Người dùng đã nộp ý tưởng");
            }

            //check student trong teammember in process OR pass
            var inTeamMember = await StudentInTeamMember((Guid)command.ReceiverId);
            if (inTeamMember)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Sinh viên đã có nhóm");
            }

            bool isSucess = await TeamCreateAsync(command);
            if (isSucess)
            {
                //noti lời mời vào nhóm
               

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

    #region phản hồi lời mời từ team bởi cá nhân
    public async Task<BusinessResult> ApproveOrRejectInvitationFromTeamByMe(InvitationUpdateCommand command)
    {
        try
        {
            // check invitation
            var invitation = await _invitationRepository.GetById(command.Id);
            if (invitation == null) return HandlerFail("Không tìm thấy lời mời!");

            // check người gửi
            var leader = await _userRepository.GetById(command.SenderId);
            if (leader == null) return HandlerFail("Không tìm thấy người gửi!");

            // check người nhận
            var student = await _userRepository.GetById(command.ReceiverId);
            if (student == null) return HandlerFail("Không tìm thấy người nhận!");

            //check dự án 
            var project = await _projectRepository.GetById(invitation.ProjectId);
            if (project == null) return HandlerFail("Không tìm thấy dự án");

            var noti = new NotificationCreateForIndividual();
            noti.UserId = invitation.SenderId;

            //reject 
            if (command.Status == InvitationStatus.Rejected)
            {
                invitation.Status = InvitationStatus.Rejected;
                await SetBaseEntityForUpdate(invitation);
                _invitationRepository.Update(invitation);

                var saveChange = await _unitOfWork.SaveChanges();
                if (!saveChange)
                {
                    return new ResponseBuilder()
                       .WithStatus(Const.FAIL_CODE)
                       .WithMessage("Đã xảy ra lỗi khi cập nhật lời mời");
                }

                //noti từ chối lời mời vào nhóm đến leader
                noti.Description = student?.Code + " đã từ chối lời mời tham gia nhóm của bạn!";
                //
                await _notificationService.CreateForIndividual(noti);

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }

            //accept
            if (command.Status == InvitationStatus.Accepted)
            {
                // Kiểm tra thành viên đã tồn tại trong nhom
                var existingMember = await _teamMemberRepository.GetByUserAndProject((Guid)invitation.ReceiverId, (Guid)invitation.ProjectId);
                if (existingMember != null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Người dùng đã là thành viên của nhóm!");
                }

                #region Xu li student accept and noti => leader
                invitation.Status = InvitationStatus.Accepted;
                await SetBaseEntityForUpdate(invitation);
                _invitationRepository.Update(invitation);

                //tao team member
                var teamMember = new TeamMember
                {
                    UserId = student.Id,
                    ProjectId = project.Id,
                    Role = TeamMemberRole.Member,
                    JoinDate = DateTime.UtcNow,
                    Status = TeamMemberStatus.Pending
                };

                await SetBaseEntityForCreation(teamMember);
                _teamMemberRepository.Add(teamMember);

                var saveChange_ = await _unitOfWork.SaveChanges();
                if (!saveChange_)
                {
                    return new ResponseBuilder()
                       .WithStatus(Const.FAIL_CODE)
                       .WithMessage(Const.FAIL_SAVE_MSG);
                }

                //noti đồng ý lời mời vào nhóm đến leader
                noti.Description = student?.Code + " đã đồng ý lời mời tham gia nhóm của bạn!";
                //
                await _notificationService.CreateForIndividual(noti);
                #endregion

                #region Xu li truong hop team du nguoi => Xu li cac loi moi va yeu cau vao nhom cua nhom
                var upcomingSemester = await _semesterRepository.GetUpComingSemester();
                if (upcomingSemester == null)
                {
                    return new ResponseBuilder()
                   .WithStatus(Const.FAIL_CODE)
                   .WithMessage("Không có kì");
                }

                //get thanh vien cua team
                var numOfMembers = 0;
                var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId((Guid)invitation.ProjectId);
                if (teamMembers != null)
                {
                    numOfMembers = teamMembers.Count();
                }

                //get idea (pending, consider, approve) cua sender - leader
                var idea = await _ideaRepository.GetIdeaNotRejectOfUserInSemester((Guid)invitation.SenderId, upcomingSemester.Id);

                //get pending invitations
                var invitationsPending = await _invitationRepository.GetInvitationsByStatusAndProjectId(InvitationStatus.Pending, (Guid)invitation.ProjectId);
                var notiForMember = new NotificationCreateForIndividual();

                //check team đủ người
                var maxTeamSize = idea?.IdeaVersions.FirstOrDefault()?.TeamSize ?? 5;

                if (numOfMembers == maxTeamSize && invitationsPending.Count > 0)
                {
                    //update status invitation => reject
                    foreach (var i in invitationsPending)
                    {
                        if (i.SenderId == invitation.SenderId)
                        {
                            i.Status = InvitationStatus.Cancel;
                        }
                        else
                        {
                            i.Status = InvitationStatus.Rejected;
                        }
                        await SetBaseEntityForUpdate(i);
                    }

                    _invitationRepository.UpdateRange(invitationsPending);
                    var saveChange1 = await _unitOfWork.SaveChanges();
                    if (!saveChange1)
                    {
                        return new ResponseBuilder()
                           .WithStatus(Const.FAIL_CODE)
                           .WithMessage(Const.FAIL_SAVE_MSG);
                    }

                    //noti nếu sender != leader
                    foreach (var i in invitationsPending)
                    {
                        if (i.SenderId != invitation.SenderId)
                        {
                            notiForMember.UserId = i.SenderId;
                            notiForMember.Description = $"Yêu cầu tham gia nhóm {project.TeamCode} của bạn đã bị từ chối!";
                            await _notificationService.CreateForIndividual(notiForMember);
                        }
                    }
                }
                #endregion

                #region Xử lí các yêu cầu vào nhóm của user
                var pendingInvitationsOfSender = await _invitationRepository.GetInvitationsBySenderIdAndStatus(student.Id, InvitationStatus.Pending);
                if (pendingInvitationsOfSender != null && pendingInvitationsOfSender.Count != 0)
                {
                    foreach (var i in pendingInvitationsOfSender)
                    {
                        i.Status = InvitationStatus.Cancel;
                        await SetBaseEntityForUpdate(i);
                    }
                    _invitationRepository.UpdateRange(pendingInvitationsOfSender);
                    
                    var saveChange = await _unitOfWork.SaveChanges();
                    if (!saveChange)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
                #endregion
            }
            return new ResponseBuilder()
                   .WithStatus(Const.FAIL_CODE)
                   .WithMessage("Trạng thái phải là chấp nhận hoặc từ chối");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(InvitationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
    #endregion

    #region phản hồi lời mời từ cá nhân bởi leader
    public async Task<BusinessResult> ApproveOrRejectInvitationFromPersonalizeByLeader(InvitationUpdateCommand command)
    {
        try
        {
            // check invitation
            var invitation = await _invitationRepository.GetById(command.Id);
            if (invitation == null) return HandlerFail("Không tìm thấy lời mời!");

            // check người gửi
            var student = await _userRepository.GetById(command.SenderId);
            if (student == null) return HandlerFail("Không tìm thấy người gửi!");

            // check người nhận
            var leader = await _userRepository.GetById(command.ReceiverId);
            if (leader == null) return HandlerFail("Không tìm thấy người nhận!");

            //check dự án 
            var project = await _projectRepository.GetById(invitation.ProjectId);
            if (project == null) return HandlerFail("Không tìm thấy dự án");

            var noti = new NotificationCreateForIndividual();
            noti.UserId = invitation.SenderId;

            //reject
            if (command.Status == InvitationStatus.Rejected)
            {
                invitation.Status = InvitationStatus.Rejected;
                _invitationRepository.Update(invitation);

                var saveResult = await _unitOfWork.SaveChanges();
                if (!saveResult)
                {
                    return HandlerFail("Đã xảy ra lỗi khi cập nhật lời mời");
                }

                //noti 
                noti.Description = $"Yêu cầu tham gia nhóm {project.TeamCode} của bạn đã bị từ chối!";
                await _notificationService.CreateForUser(noti);

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            
            if (command.Status == InvitationStatus.Cancel)
            {
                invitation.Status = InvitationStatus.Cancel;
                _invitationRepository.Update(invitation);

                var saveResult = await _unitOfWork.SaveChanges();
                if (!saveResult)
                {
                    return HandlerFail("Đã xảy ra lỗi khi cập nhật lời mời");
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Bạn đã hủy lời mời");
            }

            //accept
            if (command.Status == InvitationStatus.Accepted)
            {
                // Kiểm tra thành viên đã tồn tại trong nhom
                var existingMember = await _teamMemberRepository.GetByUserAndProject(invitation.SenderId.Value, invitation.ProjectId.Value);
                if (existingMember != null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Người dùng đã là thành viên của nhóm!");
                }

                #region Xu li leader accept and noti => student
                // Cập nhật invitation
                invitation.Status = InvitationStatus.Accepted;
                await SetBaseEntityForUpdate(invitation);
                _invitationRepository.Update(invitation);

                // Thêm thành viên mới
                var teamMember = new TeamMember
                {
                    UserId = student.Id,
                    ProjectId = project.Id,
                    Role = TeamMemberRole.Member,
                    JoinDate = DateTime.UtcNow,
                    Status = TeamMemberStatus.Pending
                };
                await SetBaseEntityForCreation(teamMember);
                _teamMemberRepository.Add(teamMember);

                var saveChange_ = await _unitOfWork.SaveChanges();
                if (!saveChange_)
                {
                    return new ResponseBuilder()
                       .WithStatus(Const.FAIL_CODE)
                       .WithMessage(Const.FAIL_SAVE_MSG);
                }

                //noti đồng ý lời mời vào nhóm đến student
                noti.Description = $"Yêu cầu tham gia nhóm {project.TeamCode} của bạn đã được chấp nhận!";
                //

                await _notificationService.CreateForIndividual(noti);
                #endregion

                #region Xu li truong hop team du nguoi => Xu li cac loi moi va yeu cau vao nhom cua nhom
                var upcomingSemester = await _semesterRepository.GetUpComingSemester();
                if (upcomingSemester == null)
                {
                    return new ResponseBuilder()
                   .WithStatus(Const.FAIL_CODE)
                   .WithMessage("Không có kì");
                }

                //get thanh vien cua team
                var numOfMembers = 0;
                var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId((Guid)invitation.ProjectId);
                if (teamMembers != null)
                {
                    numOfMembers = teamMembers.Count();
                }

                //get idea (pending, consider, approve) cua sender - leader
                var idea = await _ideaRepository.GetIdeaNotRejectOfUserInSemester((Guid)invitation.SenderId, upcomingSemester.Id);

                //get pending invitations
                var invitationsPending = await _invitationRepository.GetInvitationsByStatusAndProjectId(InvitationStatus.Pending, (Guid)invitation.ProjectId);
                var notiForMember = new NotificationCreateForIndividual();

                //check team đủ người
                var maxTeamSize = idea?.IdeaVersions.FirstOrDefault()?.TeamSize ?? 5;

                if (numOfMembers == maxTeamSize && invitationsPending != null)
                {
                    //update status invitation => reject
                    foreach (var i in invitationsPending)
                    {
                        if (i.SenderId == invitation.ReceiverId)
                        {
                            i.Status = InvitationStatus.Cancel;
                        }
                        else
                        {
                            i.Status = InvitationStatus.Rejected;
                        }
                        await SetBaseEntityForUpdate(i);
                    }

                    _invitationRepository.UpdateRange(invitationsPending);
                    var saveChange1 = await _unitOfWork.SaveChanges();
                    if (!saveChange1)
                    {
                        return new ResponseBuilder()
                           .WithStatus(Const.FAIL_CODE)
                           .WithMessage(Const.FAIL_SAVE_MSG);
                    }

                    //noti nếu sender != leader
                    foreach (var i in invitationsPending)
                    {
                        if (i.SenderId != invitation.ReceiverId)
                        {
                            notiForMember.UserId = i.SenderId;
                            notiForMember.Description = $"Yêu cầu tham gia nhóm {project.TeamCode} của bạn đã bị từ chối!";
                            await _notificationService.CreateForIndividual(notiForMember);
                        }
                    }
                }
                #endregion

                #region Xử lí các yêu cầu vào nhóm của user
                var pendingInvitationsOfSender = await _invitationRepository.GetInvitationsBySenderIdAndStatus(student.Id, InvitationStatus.Pending);
                if (pendingInvitationsOfSender != null)
                {
                    foreach (var i in pendingInvitationsOfSender)
                    {
                        i.Status = InvitationStatus.Cancel;
                        await SetBaseEntityForUpdate(i);
                    }
                    _invitationRepository.UpdateRange(pendingInvitationsOfSender);
                }

                var saveChange = await _unitOfWork.SaveChanges();
                if (!saveChange)
                {
                    return new ResponseBuilder()
                       .WithStatus(Const.FAIL_CODE)
                       .WithMessage(Const.FAIL_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
                #endregion
            }
            return new ResponseBuilder()
                   .WithStatus(Const.FAIL_CODE)
                   .WithMessage("Trạng thái của lời mời phải là chấp nhận hoặc từ chối");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
    #endregion

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
            //if (teamMember.Status != TeamMemberStatus.Failed)
            if (teamMember.Status != TeamMemberStatus.Fail2 && teamMember.Status != TeamMemberStatus.Fail1)
            {
                haveTeamMember = true;
            }
        }

        return haveTeamMember;
    }

    private async Task<bool> CheckCountMembersInTeam(Guid projectId)
    {
        var teammember = await _projectRepository.GetById(projectId);
        if (teammember == null) return false;
        var count = teammember.TeamMembers.Count(x => x.IsDeleted = false);
        return count <= teammember.TeamSize;
    }
}