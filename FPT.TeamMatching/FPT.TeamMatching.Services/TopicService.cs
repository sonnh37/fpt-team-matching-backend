using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FPT.TeamMatching.Services;

public class TopicService : BaseService<Topic>, ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStageTopicRepository _stageTopicRepositoty;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly INotificationService _notificationService;
    private readonly ITopicRequestRepository _topicRequestRepository;
    private readonly IProjectService _projectService;
    private readonly ISemesterService _semesterService;
    private readonly IUserService _userService;
    private readonly ILogger<TopicService> _logger;

    public TopicService(IMapper mapper, IUnitOfWork unitOfWork,
        IProjectService projectService,
        ISemesterService semesterService,
        INotificationService notificationService,
        IUserService userService,
        ITopicRequestService topicRequestRepository,
        ILogger<TopicService> logger
    ) : base(mapper,
        unitOfWork)
    {
        _topicRepository = unitOfWork.TopicRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _userRepository = unitOfWork.UserRepository;
        _stageTopicRepositoty = unitOfWork.StageTopicRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _topicRequestRepository = unitOfWork.TopicRequestRepository;
        _notificationService = notificationService;
        _projectService = projectService;
        _semesterService = semesterService;
        _userService = userService;
        _logger = logger;
    }

    #region Queries

    public async Task<BusinessResult> GetTopicsByUserId()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var topics = await _topicRepository.GetTopicsByUserId(userId.Value);
                var result = _mapper.Map<IList<TopicResult>>(topics);
                if (result == null)
                    return new ResponseBuilder()
                        .WithData(result)
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);

                return new ResponseBuilder()
                    .WithData(result)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    #region Create-by-student

    public async Task<BusinessResult> SubmitToMentorByStudent(TopicStudentCreatePendingCommand topicCreateModel)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            // 1. check semester's status is preparing
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return HandlerFail("Không tìm thấy kì");
            }
            if (semester.Status != SemesterStatus.Preparing)
            {
                return HandlerFail("Hiện tại không được tạo đề tài");
            }

            // 2. Validate student's existing topic vs status khac draft 
            var validationError = await ValidateStudentTopics(topicCreateModel, semester);
            if (validationError.Status != 1)
            {
                return validationError;
            }

            //3. Check xem student có bảng draft k, nếu có bảng draft thì update
            var topic = await _topicRepository.GetTopicWithStatusInSemesterOfUser((Guid)userId, semester.Id, TopicStatus.Draft);
            if (topic != null)
            {
                topic.MentorId = topicCreateModel.MentorId;
                topic.SubMentorId = topicCreateModel.SubMentorId;
                topic.VietNameseName = topicCreateModel.VietNameseName;
                topic.EnglishName = topicCreateModel.EnglishName;
                topic.Description = topicCreateModel.Description;
                topic.Abbreviation = topicCreateModel.Abbreviation;
                topic.FileUrl = topicCreateModel.FileUrl;
                //status
                topic.Status = TopicStatus.MentorPending;
                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
            }
            //k co bảng craft thì create
            else
            {
                topic = _mapper.Map<Topic>(topicCreateModel);
                topic.Id = Guid.NewGuid();
                topic.OwnerId = userId;
                topic.Status = TopicStatus.MentorPending;
                topic.SemesterId = semester.Id;
                topic.Type = TopicType.Student;
                topic.IsExistedTeam = true;
                topic.IsEnterpriseTopic = false;

                await SetBaseEntityForCreation(topic);
                _topicRepository.Add(topic);
            }
            
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Gửi đề tài không thành công");

            // 4. Create requests 
            var topicRequestForMentor = new TopicRequest
            {
                TopicId = topic.Id,
                ReviewerId = topic.MentorId,
                Status = TopicRequestStatus.Pending,
                Role = "Mentor"
            };
            await SetBaseEntityForCreation(topicRequestForMentor);
            _topicRequestRepository.Add(topicRequestForMentor);

            if (topic.SubMentorId != null)
            {
                var topicRequestForSubMentor = new TopicRequest
                {
                    TopicId = topic.Id,
                    ReviewerId = topic.SubMentorId,
                    Status = TopicRequestStatus.Pending,
                    Role = "SubMentor"
                };
                await SetBaseEntityForCreation(topicRequestForSubMentor);
                _topicRequestRepository.Add(topicRequestForSubMentor);
            }
            // 5. Save change
            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess) return HandlerFail("Gửi đề tài không thành công");
            // 6. Noti 
            await SendNotifications(topic);

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Bạn đã gửi đề tài thành công");
        }
        catch (Exception ex)
        {
            return HandlerError($"Lỗi khi gửi đề tài: {ex.Message}");
        }
    }

    #region Helper methods

    private async Task<BusinessResult> ValidateStudentTopics(TopicStudentCreatePendingCommand model,
        Semester semester)
    {
        var userId = GetUserIdFromClaims();

        var topicNotDraftOrReject = await _topicRepository.GetTopicNotRejectOfUserInSemester((Guid)userId, semester.Id);
        if (topicNotDraftOrReject != null)
        {
            if (topicNotDraftOrReject.Status == TopicStatus.ManagerApproved)
            {
                return HandlerFail("Sinh viên đã có đề tài được duyệt ở kì này");
            }
            else
            {
                return HandlerFail("Sinh viên có đề tài đang trong quá trình duyệt ở kì này");
            }
        }

        if (model.MentorId == null) return HandlerFail("Nhập field Mentor Id");
        // Check Mentor và SubMentor
        var resBool = await _userService.CheckMentorAndSubMentorSlotAvailability(model.MentorId.Value,
            model.SubMentorId);
        if (resBool.Status != 1 || resBool.Data == null) return resBool;
        var isHasSlot = resBool.Data is bool;
        if (!isHasSlot) return HandlerFail(resBool.Message);

        return new ResponseBuilder()
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_SAVE_MSG);
    }

    private async Task<Topic> CreateTopic(TopicStudentCreatePendingCommand model)
    {
        var topic = new Topic
        {
            Id = Guid.NewGuid(),
            MentorId = model.MentorId,
            SubMentorId = model.SubMentorId,
            SpecialtyId = model.SpecialtyId,
            Status = TopicStatus.MentorPending,
            OwnerId = GetUserIdFromClaims(),
            Type = TopicType.Student,
            IsExistedTeam = true,
            IsEnterpriseTopic = false
        };

        await SetBaseEntityForCreation(topic);
        _topicRepository.Add(topic);
        return topic;
    }

    //sua db
    //private async Task<TopicRegister> CreateTopicVersion(
    //    TopicStudentCreatePendingCommand model, Guid topicId, Guid stageTopicId)
    //{
    //    var topicVersion = new TopicRegister
    //    {
    //        Id = Guid.NewGuid(),
    //        TopicId = topicId,
    //        StageTopicId = stageTopicId,
    //        Version = 1,
    //        VietNamName = model.VietNamName,
    //        EnglishName = model.EnglishName,
    //        Description = model.Description,
    //        Abbreviation = model.Abbreviation,
    //        File = model.File,
    //        TeamSize = model.TeamSize,
    //    };

    //    await SetBaseEntityForCreation(topicVersion);
    //    _topicVersionRepository.Add(topicVersion);
    //    return topicVersion;
    //}

    #endregion

    #endregion

    #region Create-by-lecturer

    public async Task<BusinessResult> SubmitByLecturer(TopicLecturerCreatePendingCommand topicCreateModel)
    {
        try
        {
            var userId = GetUserIdFromClaims();

            // 1. Validate stage and semester
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return HandlerFail("Không tìm thấy kì");
            }
            if (semester.Status != SemesterStatus.Preparing)
            {
                return HandlerFail("Chưa đến thời gian đề tài");
            }
            var stageTopic = _stageTopicRepositoty.GetCurrentStageTopicBySemesterId(semester.Id);
            if (stageTopic == null)
            {
                return HandlerFail("Chưa đến thời gian nộp đề tài");
            }

            // 2. Validate lecturer-specific rules
            var validationError = await ValidateLecturerRules(topicCreateModel, semester);
            if (validationError.Status != 1)
            {
                return validationError;
            }

            //3. Check xem lecturer có bảng draft k, nếu có bảng draft thì update
            var topic = await _topicRepository.GetTopicWithStatusInSemesterOfUser((Guid)userId, semester.Id, TopicStatus.Draft);
            if (topic != null)
            {
                topic.SubMentorId = topicCreateModel.SubMentorId;
                topic.SpecialtyId = topicCreateModel.SpecialtyId;
                topic.VietNameseName = topicCreateModel.VietNameseName;
                topic.EnglishName = topicCreateModel.EnglishName;
                topic.Description = topicCreateModel.Description;
                topic.Abbreviation = topicCreateModel.Abbreviation;
                topic.IsEnterpriseTopic = topicCreateModel.IsEnterpriseTopic;
                topic.EnterpriseName = topicCreateModel.EnterpriseName;
                topic.FileUrl = topicCreateModel.FileUrl;
                //status
                topic.Status = TopicStatus.ManagerPending;
                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
            }
            //k co bảng craft thì create
            else
            {
                topic = _mapper.Map<Topic>(topicCreateModel);
                topic.Id = Guid.NewGuid();
                topic.OwnerId = userId;
                topic.Status = TopicStatus.ManagerPending;
                topic.SemesterId = semester.Id;
                topic.IsExistedTeam = false;
                topic.Type = topicCreateModel.IsEnterpriseTopic ? TopicType.Enterprise : TopicType.Lecturer;

                await SetBaseEntityForCreation(topic);
                _topicRepository.Add(topic);
            }

            // 4. Create requests 
            var topicRequestForMentor = new TopicRequest
            {
                TopicId = topic.Id,
                Status = TopicRequestStatus.Pending,
                Role = "Manager"
            };
            await SetBaseEntityForCreation(topicRequestForMentor);
            _topicRequestRepository.Add(topicRequestForMentor);

            if (topic.SubMentorId != null)
            {
                var topicRequestForSubMentor = new TopicRequest
                {
                    TopicId = topic.Id,
                    ReviewerId = topic.SubMentorId,
                    Status = TopicRequestStatus.Pending,
                    Role = "SubMentor"
                };
                await SetBaseEntityForCreation(topicRequestForSubMentor);
                _topicRequestRepository.Add(topicRequestForSubMentor);
            }
            // 5. Save change
            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess) return HandlerFail("Gửi đề tài không thành công");
            // 6. Noti 
            await SendNotifications(topic);

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Bạn đã gửi đề tài thành công");
            // 3. Create and save topic
            //var topic = await CreateLecturerTopic(topicCreateModel);
            //if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công topic");

            // 4. Create and save topic version
            //var topicVersion = await CreateLecturerTopicVersion(topicCreateModel, topic.Id, stageTopic.Id);
            //if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công topic version");

            // 5. Create requests and notifications (mentor auto-approved for lecturer)
            //sua db
            //await _topicVersionRequestService.CreateVersionRequests(topic, topicVersion.Id, semester.CriteriaFormId.Value);
            //if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công topic version request");

            // No need to notify mentor as it's auto-approved
            //sua db
            //if (topic.SubMentorId.HasValue)
            //{
            //    await SendNotifications(topic, topicVersion.Abbreviation);
            //}

            //return new ResponseBuilder()
            //    .WithStatus(Const.SUCCESS_CODE)
            //    .WithMessage("Bạn đã tạo ý tưởng thành công");
        }
        catch (Exception ex)
        {
            return HandlerError($"Lỗi khi tạo đề tài: {ex.Message}");
        }
    }

    private async Task<BusinessResult> ValidateLecturerRules(TopicLecturerCreatePendingCommand model, Semester semester)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null)
            return HandlerFailAuth();

        // Validate submentor exists if provided
        if (model.SubMentorId.HasValue)
        {
            var submentor = await _userRepository.GetById(model.SubMentorId.Value);
            if (submentor == null)
                return HandlerFail("Không tồn tại submentor");
        }

        // Check if topic needs submentor
        //var topicOnlyMentor = await _topicRepository.GetTopicsOnlyMentorOfUserInSemester((Guid)userId, semester.Id);
        //if (topicOnlyMentor.Count() > semester.LimitTopicMentorOnly && model.SubMentorId == null)
        //    return HandlerFail("Đề tài thứ " + semester.LimitTopicMentorOnly + " trở lên cần có submentor");

        // Check Mentor và SubMentor
        var resBool = await _userService.CheckMentorAndSubMentorSlotAvailability(userId.Value,model.SubMentorId);
        if (resBool.Status != 1 || resBool.Data == null) return resBool;
        var isHasSlot = resBool.Data is bool;
        if (!isHasSlot) return HandlerFail(resBool.Message);

        // Check enterprise topic requirements
        if (model.IsEnterpriseTopic && string.IsNullOrEmpty(model.EnterpriseName))
            return HandlerFail("Đề tài doanh nghiệp cần nhập tên doanh nghiệp");

        return new ResponseBuilder()
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_READ_MSG);
    }

    private async Task<Topic> CreateLecturerTopic(TopicLecturerCreatePendingCommand model)
    {
        var userId = GetUserIdFromClaims();

        var topic = new Topic
        {
            Id = Guid.NewGuid(),
            MentorId = userId,
            SubMentorId = model.SubMentorId,
            SpecialtyId = model.SpecialtyId,
            Status = TopicStatus.MentorPending,
            OwnerId = userId,
            IsExistedTeam = false,
            IsEnterpriseTopic = model.IsEnterpriseTopic,
            Type = model.IsEnterpriseTopic ? TopicType.Enterprise : TopicType.Lecturer
        };

        await SetBaseEntityForCreation(topic);
        _topicRepository.Add(topic);
        return topic;
    }

    //sua db
    //private async Task<TopicRegister> CreateLecturerTopicVersion(
    //    TopicLecturerCreatePendingCommand model, Guid topicId, Guid stageTopicId)
    //{
    //    var topicVersion = new TopicRegister
    //    {
    //        Id = Guid.NewGuid(),
    //        TopicId = topicId,
    //        StageTopicId = stageTopicId,
    //        Version = 1,
    //        VietNamName = model.VietNamName,
    //        EnglishName = model.EnglishName,
    //        Description = model.Description,
    //        Abbreviations = model.Abbreviation,
    //        File = model.File,
    //        TeamSize = model.TeamSize,
    //        EnterpriseName = model.IsEnterpriseTopic ? model.EnterpriseName : null
    //    };

    //    await SetBaseEntityForCreation(topicVersion);
    //    _topicVersionRepository.Add(topicVersion);
    //    return topicVersion;
    //}

    #endregion

    private async Task<(StageTopic? stageTopic, Semester? semester)> GetCurrentStageAndSemester()
    {
        var stageTopic = await _stageTopicRepositoty.GetCurrentStageTopic();
        if (stageTopic == null) return (null, null);

        var semester = await _semesterRepository.GetSemesterByStageTopicId(stageTopic.Id);
        return (stageTopic, semester);
    }

    private async Task SendNotifications(Topic topic)
    {
        await _notificationService.CreateForUser(new NotificationCreateForIndividual
        {
            UserId = topic.MentorId,
            Description = $"Đề tài {topic.Abbreviation} đang chờ bạn duyệt với vai trò Mentor",
        });

        if (topic.SubMentorId.HasValue)
        {
            await _notificationService.CreateForUser(new NotificationCreateForIndividual
            {
                UserId = topic.SubMentorId,
                Description = $"Đề tài {topic.Abbreviation} đang chờ bạn duyệt với vai trò SubMentor",
            });
        }
    }

    public async Task<BusinessResult> GetUserTopicsByStatus(TopicGetListForUserByStatus query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            
            if (query.StatusList.Count == 0) return HandlerFail("Nhập field trạng thái");

            var semester = await GetSemesterInCurrentWorkSpace();

            var topics = await _topicRepository.GetCurrentTopicByUserIdAndStatus(userId, semester?.Id, query.StatusList);
            var result = _mapper.Map<List<TopicResult>>(topics);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetUserTopicsByStatusWithCurrentStageTopic(TopicGetCurrentStageForUserByStatus query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerError("Không tìm thấy người dùng");

            if (query.Status == null) return HandlerFail("Nhập field trạng thái");

            var currentStageTopic = await _stageTopicRepositoty.GetCurrentStageTopic();
            if (currentStageTopic == null) return HandlerFail("Hiện tại chưa đến đợt duyệt");
            var topics = await _topicRepository.GetUserTopicsByStatusWithCurrentStageTopic(userId, query.Status,
                currentStageTopic.Id);
            var result = _mapper.Map<List<TopicResult>>(topics);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> UpdateTopic(TopicUpdateCommand topicUpdateCommand)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var project = await _topicRepository.GetById(topicUpdateCommand.Id);
            if (project == null) return HandlerFail("Không tìm thấy dự án");

            _mapper.Map(topicUpdateCommand, project);
            await SetBaseEntityForUpdate(project);
            _topicRepository.Update(project);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> UpdateStatusTopic(TopicUpdateStatusCommand command)
    {
        try
        {
            var topic = await _topicRepository.GetById(command.Id);
            if (topic == null) return HandlerFail("Không tìm thấy ý tưởng");
            topic.Status = command.Status;
            _topicRepository.Update(topic);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithData(topic)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> GetTopicsOfSupervisors<TResult>(TopicGetListOfSupervisorsQuery query)
        where TResult : BaseResult
    {
        try
        {
            List<TResult>? results;

            var (data, total) = await _topicRepository.GetTopicsOfSupervisors(query);

            results = _mapper.Map<List<TResult>>(data);

            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    #endregion

    private const double APPROVAL_THRESHOLD = 0.5;
    private const int MIN_REVIEWERS = 2;
    private const int RANDOM_SUFFIX_MIN = 1000;
    private const int RANDOM_SUFFIX_MAX = 9999;
    private const int MAX_TEAMNAME_LENGTH = 15;

    public async Task<BusinessResult> AutoUpdateTopicStatus()
    {
        try
        {
            var topics = await _topicRepository.GetTopicWithResultDateIsToday();
            var stageCurrent = await _stageTopicRepositoty.GetCurrentStageTopic();

            if (topics?.Count > 0 && stageCurrent != null)
            {
                foreach (var topic in topics)
                {
                    //sua db
                    //await EvaluateTopicByAverageScore(topic, stageCurrent);
                }
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    //private async Task EvaluateTopicByAverageScore(Topic topic, StageTopic stageTopic)
    //{
    //    var totalCouncils = await _topicVersionRequestRepository.CountCouncilsForTopic(topic.Id);
    //    var requiredReviewers = stageTopic.NumberReviewer ?? MIN_REVIEWERS;

    //    if (totalCouncils < requiredReviewers)
    //        return;

    //    var totalPending =
    //        await _topicVersionRequestRepository.CountStatusCouncilsForTopic(topic.Id, TopicVersionRequestStatus.Pending);
    //    var totalApproved =
    //        await _topicVersionRequestRepository.CountStatusCouncilsForTopic(topic.Id, TopicVersionRequestStatus.Approved);
    //    var totalRejected =
    //        await _topicVersionRequestRepository.CountStatusCouncilsForTopic(topic.Id, TopicVersionRequestStatus.Rejected);
    //    var totalConsider =
    //        await _topicVersionRequestRepository.CountStatusCouncilsForTopic(topic.Id, TopicVersionRequestStatus.Consider);

    //    if (totalPending > 0)
    //    {
    //        await UpdatePendingToRejected(topic.Id);
    //    }


    //    var totalScore = (totalApproved * 1.0) + (totalConsider * 0.5);
    //    var averageScore = totalScore / totalCouncils;

    //    var status = averageScore switch
    //    {
    //        > APPROVAL_THRESHOLD => TopicStatus.Approved,
    //        < APPROVAL_THRESHOLD => TopicStatus.Rejected,
    //        _ => TopicStatus.ConsiderByCouncil
    //    };

    //    await UpdateTopicStatus(topic, stageTopic, status);
    //}

    //sua db
    //private async Task UpdatePendingToRejected(Guid topicId)
    //{
    //    var pendingRequests = await _topicVersionRequestRepository.GetQueryable().Include(m => m.TopicVersion)
    //        .Where(x => x.TopicVersion != null && x.TopicVersion.TopicId == topicId && x.Role == "Council" &&
    //                    x.Status == TopicVersionRequestStatus.Pending)
    //        .ToListAsync();

    //    foreach (var request in pendingRequests)
    //    {
    //        request.Status = TopicVersionRequestStatus.Rejected;
    //        await SetBaseEntityForUpdate(request);
    //    }

    //    await _unitOfWork.SaveChanges();
    //}

    private async Task UpdateTopicStatus(Topic topic, StageTopic stageTopicCurrent, TopicStatus status)
    {
        try
        {
            //sua db
            //var topicVersionCurrent = _topicVersionRepository.GetQueryable()
            //    .Where(m => m.TopicId == topic.Id &&
            //                !m.IsDeleted)
            //    .OrderByDescending(m => m.Version)
            //    .FirstOrDefault();
            //if (topicVersionCurrent == null) return;

            //if (status == TopicStatus.Approved)
            //{
            //    await HandleApprovedTopic(topic, topicVersionCurrent, stageTopicCurrent);
            //}

            //await UpdateTopicCore(topic, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating topic {topic?.Id}");
            throw;
        }
    }

    private async Task HandleApprovedTopic(Topic topic, Topic topicVersion, StageTopic stageTopic)
    {
        //sua db
        //var topicVersionsOfTopic = await _topicVersionRepository.GetTopicVersionsByTopicId(topicVersion.TopicId.Value);
        //var topicVersionListId = topicVersionsOfTopic.Select(m => m.Id).ToList().ConvertAll<Guid?>(x => x);
        //var existingTopics = await _unitOfWork.TopicRepository.GetTopicByTopicVersionId(topicVersionListId);

        //if (existingTopics.Count > 0)
        //{
        //    var topic = existingTopics[0];
        //    topic.TopicVersionId = topicVersion.Id;
        //    _unitOfWork.TopicRepository.Update(topic);
        //    await _unitOfWork.SaveChanges();
        //}

        // Kiểm tra xem TopicVersion đã có Topic chưa
        //var existingTopic = await _topicRepository.GetQueryable()
        //    .SingleOrDefaultAsync(m => m.TopicVersionId == topicVersion.Id);

        //var existingTopicFilterds = existingTopics.Where(m => m.Id != existingTopic?.Id).ToList();
        //if (existingTopicFilterds.Count != 0)
        //{
        //    // remove
        //    _unitOfWork.TopicRepository.DeleteRangePermanently(existingTopicFilterds);
        //    await _unitOfWork.SaveChanges();
        //}

        TopicResult? topicResult = null;

        //if (existingTopic == null)
        //{
        //    topicResult = await CreateTopicForTopic(topicVersion, stageTopic);
        //    if (topicResult == null) return;
        //}
        //else
        //{
        //    topicResult = new TopicOldResult
        //    {
        //        Id = existingTopic.Id,
        //        TopicCode = existingTopic.TopicCode
        //    };
        //}

        // Mentor return
        //var owner = await _unitOfWork.UserRepository.GetQueryable(m => m.Id == topic.OwnerId)
        //    .Include(e => e.UserXRoles).ThenInclude(e => e.Role).SingleOrDefaultAsync();
        //var semesterGetUpComing = await _semesterRepository.GetUpComingSemester();

        //// Nếu như kì sắp tới mà mentor chưa đc role student thì return
        //if (owner?.UserXRoles?.Any(e => e.Role?.RoleName == "Mentor" && semesterGetUpComing?.Id == e.SemesterId) ==
        //    true)
        //    return;

        //var existedProject = await _projectRepository.GetProjectByLeaderId(topic.OwnerId);

        //if (existedProject == null)
        //{
        //    await CreateNewProject(topic, topicVersion, stageTopic, topicResult);
        //}
        //else
        //{
        //    await UpdateExistingProject(existedProject, stageTopic, topicResult);
        //}
    }

    public async Task<BusinessResult> GetTopicsOfReviewerByRolesAndStatus<TResult>(
        TopicRequestGetListByStatusAndRoleQuery query) where TResult : BaseResult
    {
        try
        {
            var userIdClaims = GetUserIdFromClaims();
            var userId = userIdClaims.Value;
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return HandlerFail("Không tìm thấy kì");
            }
            
            var (data, total) =
                await _topicRepository.GetTopicsOfReviewerByRolesAndStatus(query,
                    userId, semester.Id);

            var results = _mapper.Map<List<TResult>>(data);

            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    //sua db
    //private async Task<TopicResult?> CreateTopicForTopic(TopicRe topicVersion, StageTopic stageTopic)
    //{
    //try
    //{
    //    var newTopicCode = await _semesterService.GenerateNewTopicCode(stageTopic.SemesterId);
    //    var codeExist = _topicRepository.IsExistedTopicCode(newTopicCode);
    //    if (codeExist) return null;

    //    var topicCreateCommand = new TopicCreateCommand
    //    {
    //        TopicVersionId = topicVersion.Id,
    //        TopicCode = newTopicCode
    //    };

    //    var res = await _topicService.CreateOrUpdate<TopicOldResult>(topicCreateCommand);

    //    return res.Status == 1 ? res.Data as TopicOldResult : null;
    //}
    //catch (Exception ex)
    //{
    //    _logger.LogError(ex, $"Error creating topic for topic version {topicVersion.Id}");
    //    return null;
    //}
    //}

    //sua db
    //private async Task CreateNewProject(Topic topic, TopicRegister topicVersion, StageTopic stageTopic,
    //    TopicOldResult topicResult)
    //{
    //    try
    //    {
    //        var newTeamCode = await _semesterService.GenerateNewTeamCode(stageTopic.SemesterId);
    //        var truncatedName = topicVersion.EnglishName?.Length > MAX_TEAMNAME_LENGTH
    //            ? topicVersion.EnglishName.Substring(0, MAX_TEAMNAME_LENGTH)
    //            : topicVersion.EnglishName;

    //        var command = new ProjectCreateCommand
    //        {
    //            LeaderId = topic.OwnerId,
    //            TopicId = topicResult.Id,
    //            TeamCode = newTeamCode,
    //            TeamName = $"{truncatedName}-{Random.Shared.Next(RANDOM_SUFFIX_MIN, RANDOM_SUFFIX_MAX)}",
    //            TeamSize = 1,
    //            Status = ProjectStatus.Pending
    //        };

    //        await _projectService.CreateProjectAndTeammemberForAuto(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error creating new project for topic {topic.Id}");
    //        throw;
    //    }
    //}

    private async Task UpdateExistingProject(Project existingProject, StageTopic stageTopic, TopicResult topicResult)
    {
        try
        {
            existingProject.TopicId = topicResult.Id;
            if (string.IsNullOrEmpty(existingProject.TeamCode))
            {
                existingProject.TeamCode = await _semesterService.GenerateNewTeamCode();
            }
            existingProject.Status = ProjectStatus.Pending;
            await SetBaseEntityForUpdate(existingProject);
            _projectRepository.Update(existingProject);

            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                _logger.LogWarning($"Failed to update existing project {existingProject.Id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating existing project {existingProject.Id}");
            throw;
        }
    }

    private async Task UpdateTopicCore(Topic topic, TopicStatus status)
    {
        try
        {
            topic.Owner = null;
            topic.Mentor = null;
            topic.SubMentor = null;
            topic.Specialty = null;
            // topic.TopicVersions = new List<TopicVersion>();
            topic.Status = status;

            await SetBaseEntityForUpdate(topic);
            _topicRepository.Update(topic);
            await _unitOfWork.SaveChanges();

            var noti = new NotificationCreateForIndividual()
            {
                UserId = topic.OwnerId,
                Description = "Kết quả đề tài đã được công bố. Hãy kiểm tra kết quả!"
            };
            await _notificationService.CreateForUser(noti);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating core topic {topic.Id}");
            throw;
        }
    }

    public async Task<BusinessResult> UpdateWhenSemesterStart()
    {
        try
        {
            //get projects cua ki den ngay bat dau ki - ngay khoa
            var semester = await _semesterRepository.GetSemesterStartToday();
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có kì bắt đầu ở ngày hiện tại");
            }

            //get topic của kì với status khác approve => reject
            var topics = await _topicRepository.GetTopicNotApproveInSemester(semester.Id);
            if (topics != null)
            {
                foreach (var topic in topics)
                {
                    topic.Status = TopicStatus.ManagerRejected;
                    await SetBaseEntityForUpdate(topic);
                }

                _topicRepository.UpdateRange(topics);

                //sua db
                //get topic version request (role mentor) của kì với status khác approve => reject
                //var topicVersionRequests =
                //    await _topicVersionRequestRepository.GetRoleMentorNotApproveInSemester(semester.Id);
                //if (topicVersionRequests != null)
                //{
                //    foreach (var topicVersionRequest in topicVersionRequests)
                //    {
                //        topicVersionRequest.Status = TopicVersionRequestStatus.Rejected;
                //        await SetBaseEntityForUpdate(topicVersionRequest);
                //    }

                //    _topicVersionRequestRepository.UpdateRange(topicVersionRequests);
                //}
            }

            //project k co topic => canceled
            var projectsWithNoTopic =
                await _projectRepository.GetPendingProjectsWithNoTopicStartingBySemesterId(semester.Id);
            if (projectsWithNoTopic != null)
            {
                foreach (var project in projectsWithNoTopic)
                {
                    project.Status = ProjectStatus.Canceled;
                    await SetBaseEntityForUpdate(project);
                }

                _projectRepository.UpdateRange(projectsWithNoTopic);
            }

            //get project của kì với status pending check xem topic approve có team size = với teamsize của nhóm k
            var projectsWithTopic =
                await _projectRepository.GetPendingProjectsWithTopicStartingBySemesterId(semester.Id);
            if (projectsWithTopic != null)
            {
                foreach (var project in projectsWithTopic)
                {
                    //sua db
                    //var topicVersion = await _topicVersionRepository.GetLastTopicVersionByTopicId((Guid)project.TopicId);
                    //if (topicVersion != null)
                    //{
                    //    //get team members
                    //    var members = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                    //    var numOfMembers = 0;
                    //    if (members != null)
                    //    {
                    //        numOfMembers = members.Count();
                    //    }

                    //    //teamsize != voi teamsize cua nhom, project status => canceled
                    //    if (topicVersion.TeamSize != numOfMembers)
                    //    {
                    //        project.Status = ProjectStatus.Canceled;
                    //    }

                    //    //teamsize = voi teamsize cua nhom, project status => inprogress, teammember => inprogress
                    //    else
                    //    {
                    //        project.Status = ProjectStatus.InProgress;

                    //        foreach (var member in members)
                    //        {
                    //            member.Status = TeamMemberStatus.InProgress;
                    //            await SetBaseEntityForUpdate(member);
                    //        }

                    //        _teamMemberRepository.UpdateRange(members);
                    //    }

                    //    await SetBaseEntityForUpdate(project);
                    //}
                }

                _projectRepository.UpdateRange(projectsWithTopic);
            }

            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                var notiSystemForStudent = new NotificationCreateForRoleBased
                {
                    Description =
                        "Hôm nay là ngày bắt đầu kì học. Tất cả đề tài chưa duyệt và nhóm thiếu thành viên đã bị tự động hủy. Vui lòng kiểm tra!",
                    Role = "Student"
                };
                await _notificationService.CreateForRoleBased(notiSystemForStudent);
                var notiSystemForLecturer = new NotificationCreateForRoleBased
                {
                    Description =
                        "Hôm nay là ngày bắt đầu kì học. Tất cả đề tài chưa duyệt và nhóm thiếu thành viên đã bị tự động hủy. Vui lòng kiểm tra!",
                    Role = "Lecturer"
                };
                await _notificationService.CreateForRoleBased(notiSystemForLecturer);
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Cập nhật các dự án đến ngày bắt đầu thành công");
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Đã xảy ra lỗi khi cập nhật các dự án đến ngày bắt đầu");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetTopicsForMentor(TopicGetListForMentorQuery query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFailAuth();

            var (data, total) = await _topicRepository.GetTopicsForMentor(query, userId.Value);
            var results = _mapper.Map<List<TopicResult>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetApprovedTopicsDoNotHaveTeam()
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy kì");
            }
            var topics = await _topicRepository.GetApprovedTopicsDoNotHaveTeamInSemester(semester.Id);
            return new ResponseBuilder()
                .WithData(topics)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}