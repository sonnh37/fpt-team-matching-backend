using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorTopicRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorTopicRequests;
using DocumentFormat.OpenXml.Drawing;

namespace FPT.TeamMatching.Services
{
    public class MentorTopicRequestService : BaseService<MentorTopicRequest>, IMentorTopicRequestService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IMentorTopicRequestRepository _mentorIdeaRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly ISemesterService _semesterService;

        public MentorTopicRequestService(IMapper mapper, IUnitOfWork unitOfWork,
            ISemesterService semesterService,
            INotificationService notificationService) : base(mapper, unitOfWork)
        {
            _projectRepository = _unitOfWork.ProjectRepository;
            _topicRepository = _unitOfWork.TopicRepository;
            _teamMemberRepository = _unitOfWork.TeamMemberRepository;
            _mentorIdeaRequestRepository = _unitOfWork.MentorTopicRequestRepository;
            _notificationService = notificationService;
            _semesterService = semesterService;
        }

        public async Task<BusinessResult> StudentRequestTopic(StudentRequest request)
        {
            try
            {
                //check topic exist
                var userId = GetUserIdFromClaims();
                if (userId == null) return HandlerFailAuth();
                var projectOfUserCurrent = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
                if (projectOfUserCurrent == null) return HandlerFail("Vui lòng tạo nhóm trước khi gửi yêu cầu");

                //check co nhom
                var teamMember = await _teamMemberRepository.GetPendingTeamMemberOfUser((Guid)userId);
                if (teamMember.Role != TeamMemberRole.Leader) return HandlerFail("Chỉ nhóm trưởng được xin đề tài");
                var project = await _projectRepository.GetProjectWithStatusByLeaderId((Guid)teamMember.UserId, [ProjectStatus.Pending, ProjectStatus.Forming, ProjectStatus.InProgress]);
                if (project == null) return HandlerFail("Cần có nhóm trước khi xin đề tài");
                if (project.TopicId != null) return HandlerFail("Nhóm bạn đã có đề tài");

                var hasUserSentRequest = projectOfUserCurrent.MentorTopicRequests.Any(m => m.ProjectId == projectOfUserCurrent.Id && m.Status != MentorTopicRequestStatus.Rejected);
                if (hasUserSentRequest) return HandlerFail("Bạn đã gửi yêu cầu cho đề tài này");

                var topic = await _unitOfWork.TopicRepository.GetById(request.TopicId, true);
                if (topic == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy đề tài");
                }

                var entity = new MentorTopicRequest
                {
                    ProjectId = projectOfUserCurrent.Id,
                    TopicId = request.TopicId,
                    Status = MentorTopicRequestStatus.Pending
                };
                await SetBaseEntityForUpdate(entity);
                _mentorIdeaRequestRepository.Add(entity);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
                    //noti cho mentor
                    var noti = new NotificationCreateForIndividual
                    {
                        UserId = topic.MentorId,
                        Description = "Nhóm " + projectOfUserCurrent.TeamName + " đã gửi yêu cầu sử dụng đề tài đến bạn"
                    };
                    await _notificationService.CreateForUser(noti);

                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_DELETE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(MentorTopicRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetUserMentorTopicRequests(MentorTopicRequestGetAllQuery query)
        {
            try
            {
                var userIdClaim = GetUserIdFromClaims();

                if (userIdClaim == null)
                    return HandlerFailAuth();

                var userId = userIdClaim.Value;
                var (data, total) = await _mentorIdeaRequestRepository.GetUserMentorTopicRequests(query, userId);
                var results = _mapper.Map<List<MentorTopicRequestResult>>(data);
                var response = new QueryResult(query, results, total);

                return new ResponseBuilder()
                    .WithData(response)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(MentorTopicRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetMentorMentorTopicRequests(MentorTopicRequestGetAllQuery query)
        {
            try
            {
                var userIdClaim = GetUserIdFromClaims();
                if (userIdClaim == null)
                    return HandlerFailAuth();

                var userId = userIdClaim.Value;
                var (data, total) = await _mentorIdeaRequestRepository.GetMentorMentorTopicRequests(query, userId);
                var results = _mapper.Map<List<MentorTopicRequestResult>>(data);
                var response = new QueryResult(query, results, total);

                return new ResponseBuilder()
                    .WithData(response)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(MentorTopicRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> UpdateMentorTopicRequestStatus(MentorTopicRequestUpdateCommand request)
        {
            try
            {
                if (request.ProjectId == null || request.TopicId == null || request.Status == null)
                    return HandlerFail("Nhập không đủ field");

                var mentorIdeaRequest = await _mentorIdeaRequestRepository.GetById(request.Id);
                var project = await _projectRepository.GetById(request.ProjectId);
                var topic = await _unitOfWork.TopicRepository.GetById(request.TopicId, true);
                if (mentorIdeaRequest == null) return HandlerFail("Không tìm thấy request");
                if (project == null) return HandlerFail("Không tìm thấy team");

                if (topic == null) return HandlerFail("Không tìm thấy đề tài");

                if (topic == null) return HandlerFail("Không tìm thấy thông tin của đề tài");

                if (topic.MentorId == null) return HandlerFail("Đề tài không có Mentor");

                var mentor = await _unitOfWork.UserRepository.GetById(topic.MentorId.Value);

                if (mentor == null) return HandlerFail("Không tìm thấy Mentor");

                //nếu reject -> update 1 reject
                if (request.Status == MentorTopicRequestStatus.Rejected)
                {
                    mentorIdeaRequest.Status = MentorTopicRequestStatus.Rejected;
                    await SetBaseEntityForUpdate(mentorIdeaRequest);
                    _mentorIdeaRequestRepository.Update(mentorIdeaRequest);
                    if (!await _unitOfWork.SaveChanges())
                        return HandlerFail("Xảy ra lỗi khi cập nhật trạng thái của yêu cầu");

                    //noti cho nhom 
                    var noti = new NotificationCreateForTeam
                    {
                        Description = "Mentor " + mentor.Code +
                                      "  đã duyệt yêu cầu sử dụng đề tài của nhóm bạn. Hãy kiểm tra!",
                        ProjectId = project.Id
                    };

                    await _notificationService.CreateForTeam(noti);
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                //nếu apprrove -> update 1 approve, others reject
                if (request.Status == MentorTopicRequestStatus.Approved)
                {
                    //lấy ra tất cả request cua idea do
                    var mentorIdeaRequests = await _mentorIdeaRequestRepository.GetByTopicId((Guid)request.TopicId);
                    if (mentorIdeaRequests != null)
                    {
                        //update
                        foreach (var item in mentorIdeaRequests)
                        {
                            item.Status = (item.Id == request.Id)
                                ? MentorTopicRequestStatus.Approved
                                : MentorTopicRequestStatus.Rejected;
                            await SetBaseEntityForUpdate(item);
                            _mentorIdeaRequestRepository.Update(item);

                            if (!await _unitOfWork.SaveChanges())
                                return HandlerFail("Xảy ra lỗi khi cập nhật trạng thái của các yêu cầu còn lại");

                            //noti cho nhom 
                            var noti2 = new NotificationCreateForTeam
                            {
                                Description = "Mentor " + mentor.Code +
                                              "  đã duyệt yêu cầu sử dụng đề tài của nhóm bạn. Hãy kiểm tra!",
                                ProjectId = project.Id
                            };

                            await _notificationService.CreateForTeam(noti2);
                        }
                    }

                    //idea: cap nhat isExistedTeam
                    var idea = await _topicRepository.GetById(topic.Id);
                    idea.IsExistedTeam = true;
                    await SetBaseEntityForUpdate(idea);
                    _topicRepository.Update(idea);
                    if (!await _unitOfWork.SaveChanges()) return HandlerFail("Xảy ra lỗi khi cập nhật đề tài");

                    var semester = await _unitOfWork.SemesterRepository.GetUpComingSemester();
                    if (semester != null)
                    {
                        if (string.IsNullOrEmpty(project.TeamCode))
                        {
                            project.TeamCode = await _semesterService.GenerateNewTeamCode();
                        }
                    }

                    //gan topicId vao project
                    project.TopicId = request.TopicId;

                    await SetBaseEntityForUpdate(project);
                    _projectRepository.Update(project);
                    if (!await _unitOfWork.SaveChanges()) return HandlerFail("Đã xảy ra lỗi khi cập nhật đề tài");

                    //noti cho nhom 
                    var noti = new NotificationCreateForTeam
                    {
                        Description = "Mentor " + mentor.Code +
                                      "  đã duyệt yêu cầu sử dụng đề tài của nhóm bạn. Hãy kiểm tra!",
                        ProjectId = project.Id,
                    };

                    await _notificationService.CreateForTeam(noti);

                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage("Đã phản hồi thành công");
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(MentorTopicRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}