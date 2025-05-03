using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FPT.TeamMatching.Services;

public class IdeaService : BaseService<Idea>, IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStageIdeaRepositoty _stageIdeaRepositoty;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly INotificationService _notificationService;
    private readonly IIdeaVersionRepository _ideaVersionRepository;
    private readonly IIdeaVersionRequestRepository _ideaVersionRequestRepository;
    private readonly IIdeaVersionRequestService _ideaVersionRequestService;
    private readonly ITopicRepository _topicRepository;
    private readonly IProjectService _projectService;
    private readonly ISemesterService _semesterService;
    private readonly ITopicService _topicService;
    private readonly IUserService _userService;
    private readonly ILogger<IdeaService> _logger;

    public IdeaService(IMapper mapper, IUnitOfWork unitOfWork,
        IProjectService projectService,
        ISemesterService semesterService,
        ITopicService topicService,
        INotificationService notificationService,
        IUserService userService,
        IIdeaVersionRequestService ideaVersionRequestService,
        ILogger<IdeaService> logger
    ) : base(mapper,
        unitOfWork)
    {
        _ideaRepository = unitOfWork.IdeaRepository;
        _ideaVersionRequestService = ideaVersionRequestService;
        _ideaVersionRequestRepository = unitOfWork.IdeaVersionRequestRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _userRepository = unitOfWork.UserRepository;
        _stageIdeaRepositoty = unitOfWork.StageIdeaRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _ideaVersionRepository = unitOfWork.IdeaVersionRepository;
        _topicRepository = unitOfWork.TopicRepository;
        _notificationService = notificationService;
        _projectService = projectService;
        _semesterService = semesterService;
        _topicService = topicService;
        _userService = userService;
        _logger = logger;
    }

    #region Queries

    public async Task<BusinessResult> GetIdeasByUserId()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var ideas = await _ideaRepository.GetIdeasByUserId(userId.Value);
                var result = _mapper.Map<IList<IdeaResult>>(ideas);
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
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    #region Create-by-student
    public async Task<BusinessResult> CreatePendingByStudent(IdeaStudentCreatePendingCommand ideaCreateModel)
    {
        try
        {
            // 1. Validate stage and semester
            var (stageIdea, semester) = await GetCurrentStageAndSemester();
            if (stageIdea == null || semester == null)
            {
                return HandlerFail("Không tìm thấy đợt duyệt hoặc kì tương ứng");
            }

            // 2. Validate student's existing ideas
            var validationError = await ValidateStudentIdeas(ideaCreateModel, stageIdea.Id, semester.Id);
            if (validationError.Status != 1)
            {
                return validationError;
            }

            // 3. Create and save idea
            var idea = await CreateIdea(ideaCreateModel);
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công idea");

            // 4. Create and save idea version
            var ideaVersion = await CreateIdeaVersion(ideaCreateModel, idea.Id, stageIdea.Id);
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công idea version");


            // 5. Create requests and notifications
            await _ideaVersionRequestService.CreateVersionRequests(idea, ideaVersion.Id, semester.CriteriaFormId.Value);
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công idea version request");
            await SendNotifications(idea, ideaVersion.Abbreviations);

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Bạn đã tạo ý tưởng thành công");
        }
        catch (Exception ex)
        {
            return HandlerError($"Lỗi khi tạo đề tài: {ex.Message}");
        }
    }

    #region Helper methods
    private async Task<BusinessResult> ValidateStudentIdeas(IdeaStudentCreatePendingCommand model, Guid stageIdeaId,
        Guid semesterId)
    {
        var userId = GetUserIdFromClaims();

        if (await _ideaRepository.GetIdeaApproveInSemesterOfUser(userId, semesterId) != null)
        {
            return HandlerFail("Sinh viên đã có đề tài được duyệt trong kì này");
        }

        if (await _ideaRepository.GetIdeaPendingInStageIdeaOfUser(userId, stageIdeaId) != null)
        {
            return HandlerFail("Sinh viên có đề tài đang trong quá trình duyệt ở kì này");
        }

        if (model.MentorId == null) return HandlerFail("Bat buoc phai nhap mentorId");
        // Check Mentor và sub
        var resBool = await _userService.CheckMentorAndSubMentorSlotAvailability(model.MentorId.Value,
            model.SubMentorId);
        if (resBool.Status != 1 || resBool.Data == null) return resBool;
        var isHasSlot = resBool.Data is bool;
        if (!isHasSlot) return HandlerFail(resBool.Message);

        return new ResponseBuilder()
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_SAVE_MSG);
    }

    private async Task<Idea> CreateIdea(IdeaStudentCreatePendingCommand model)
    {
        var idea = new Idea
        {
            Id = Guid.NewGuid(),
            MentorId = model.MentorId,
            SubMentorId = model.SubMentorId,
            SpecialtyId = model.SpecialtyId,
            Status = IdeaStatus.Pending,
            OwnerId = GetUserIdFromClaims(),
            Type = IdeaType.Student,
            IsExistedTeam = true,
            IsEnterpriseTopic = false
        };

        await SetBaseEntityForCreation(idea);
        _ideaRepository.Add(idea);
        return idea;
    }

    private async Task<IdeaVersion> CreateIdeaVersion(
        IdeaStudentCreatePendingCommand model, Guid ideaId, Guid stageIdeaId)
    {
        var ideaVersion = new IdeaVersion
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaId,
            StageIdeaId = stageIdeaId,
            Version = 1,
            VietNamName = model.VietNamName,
            EnglishName = model.EnglishName,
            Description = model.Description,
            Abbreviations = model.Abbreviations,
            File = model.File,
            TeamSize = model.TeamSize,
        };

        await SetBaseEntityForCreation(ideaVersion);
        _ideaVersionRepository.Add(ideaVersion);
        return ideaVersion;
    }

    #endregion
    #endregion

    #region Create-by-lecturer

    public async Task<BusinessResult> CreatePendingByLecturer(IdeaLecturerCreatePendingCommand ideaCreateModel)
    {
        try
        {
            // 1. Validate stage and semester
            var (stageIdea, semester) = await GetCurrentStageAndSemester();
            if (stageIdea == null || semester == null) return HandlerFail("Không tìm thấy đợt duyệt hoặc kì tương ứng");

            // 2. Validate lecturer-specific rules
            var validationError = await ValidateLecturerRules(ideaCreateModel);
            if (validationError.Status != 1)
            {
                return validationError;
            }

            // 3. Create and save idea
            var idea = await CreateLecturerIdea(ideaCreateModel);
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công idea");

            // 4. Create and save idea version
            var ideaVersion = await CreateLecturerIdeaVersion(ideaCreateModel, idea.Id, stageIdea.Id);
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công idea version");

            // 5. Create requests and notifications (mentor auto-approved for lecturer)
            await _ideaVersionRequestService.CreateVersionRequests(idea, ideaVersion.Id, semester.CriteriaFormId.Value);
            if (!await _unitOfWork.SaveChanges()) return HandlerFail("Lưu không thành công idea version request");

            // No need to notify mentor as it's auto-approved
            if (idea.SubMentorId.HasValue)
            {
                await SendNotifications(idea, ideaVersion.Abbreviations);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Bạn đã tạo ý tưởng thành công");
        }
        catch (Exception ex)
        {
            return HandlerError($"Lỗi khi tạo đề tài: {ex.Message}");
        }
    }

    private async Task<BusinessResult> ValidateLecturerRules(IdeaLecturerCreatePendingCommand model)
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

        // Check if this is 5th+ idea and needs submentor
        // var numberOfIdeas = await _ideaRepository.NumberOfIdeaMentorOrOwner(userId.Value);
        // if (numberOfIdeas > 4 && model.SubMentorId == null)
        //     return HandlerFail("Đề tài thứ 5 trở lên cần có submentor");

        // Check Mentor và sub
        var resBool = await _userService.CheckMentorAndSubMentorSlotAvailability(userId.Value,
            model.SubMentorId);
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

    private async Task<Idea> CreateLecturerIdea(IdeaLecturerCreatePendingCommand model)
    {
        var userId = GetUserIdFromClaims();

        var idea = new Idea
        {
            Id = Guid.NewGuid(),
            MentorId = userId,
            SubMentorId = model.SubMentorId,
            SpecialtyId = model.SpecialtyId,
            Status = IdeaStatus.Pending,
            OwnerId = userId,
            IsExistedTeam = false,
            IsEnterpriseTopic = model.IsEnterpriseTopic,
            Type = model.IsEnterpriseTopic ? IdeaType.Enterprise : IdeaType.Lecturer
        };

        await SetBaseEntityForCreation(idea);
        _ideaRepository.Add(idea);
        return idea;
    }

    private async Task<IdeaVersion> CreateLecturerIdeaVersion(
        IdeaLecturerCreatePendingCommand model, Guid ideaId, Guid stageIdeaId)
    {
        var ideaVersion = new IdeaVersion
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaId,
            StageIdeaId = stageIdeaId,
            Version = 1,
            VietNamName = model.VietNamName,
            EnglishName = model.EnglishName,
            Description = model.Description,
            Abbreviations = model.Abbreviations,
            File = model.File,
            TeamSize = model.TeamSize,
            EnterpriseName = model.IsEnterpriseTopic ? model.EnterpriseName : null
        };

        await SetBaseEntityForCreation(ideaVersion);
        _ideaVersionRepository.Add(ideaVersion);
        return ideaVersion;
    }


    #endregion

    private async Task<(StageIdea? stageIdea, Semester? semester)> GetCurrentStageAndSemester()
    {
        var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
        if (stageIdea == null) return (null, null);

        var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
        return (stageIdea, semester);
    }

    private async Task SendNotifications(Idea idea, string abbreviations)
    {
        await _notificationService.CreateForUser(new NotificationCreateForIndividual
        {
            UserId = idea.MentorId,
            Description = $"Đề tài {abbreviations} đang chờ bạn duyệt với vai trò Mentor",
        });

        if (idea.SubMentorId.HasValue)
        {
            await _notificationService.CreateForUser(new NotificationCreateForIndividual
            {
                UserId = idea.SubMentorId.Value,
                Description = $"Đề tài {abbreviations} đang chờ bạn duyệt với vai trò SubMentor",
            });
        }
    }

    public async Task<BusinessResult> GetUserIdeasByStatus(IdeaGetListForUserByStatus query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerError("User does not exist");

            if (query.Status == null) return HandlerFail("Status cannot be null");

            var ideas = await _ideaRepository.GetCurrentIdeaByUserIdAndStatus(userId.Value, query.Status.Value);
            var result = _mapper.Map<List<IdeaResult>>(ideas);
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
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetUserIdeasByStatusWithCurrentStageIdea(IdeaGetCurrentStageForUserByStatus query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerError("User does not exist");

            if (query.Status == null) return HandlerFail("Status cannot be null");

            var currentStageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
            if (currentStageIdea == null) return HandlerFail("Current stage Idea does not exist");
            var ideas = await _ideaRepository.GetUserIdeasByStatusWithCurrentStageIdea(userId, query.Status,
                currentStageIdea.Id);
            var result = _mapper.Map<List<IdeaResult>>(ideas);
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
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> UpdateIdea(IdeaUpdateCommand ideaUpdateCommand)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var project = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
            if (project == null) return HandlerFail("Not found project");

            var command = _mapper.Map<Idea>(ideaUpdateCommand);
            await SetBaseEntityForUpdate(command);
            _ideaRepository.Update(command);
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

    public async Task<BusinessResult> UpdateStatusIdea(IdeaUpdateStatusCommand command)
    {
        try
        {
            var idea = await _ideaRepository.GetById(command.Id);
            if (idea == null) return HandlerFail("Not found idea");
            idea.Status = command.Status;
            _ideaRepository.Update(idea);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithData(idea)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> GetIdeasOfSupervisors<TResult>(IdeaGetListOfSupervisorsQuery query)
        where TResult : BaseResult
    {
        try
        {
            List<TResult>? results;

            var (data, total) = await _ideaRepository.GetIdeasOfSupervisors(query);

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

    public async Task<BusinessResult> AutoUpdateIdeaStatus()
    {
        try
        {
            var ideas = await _ideaRepository.GetIdeaWithResultDateIsToday();
            var stageCurrent = await _stageIdeaRepositoty.GetCurrentStageIdea();

            if (ideas?.Count > 0 && stageCurrent != null)
            {
                foreach (var idea in ideas)
                {
                    await EvaluateIdeaByAverageScore(idea, stageCurrent);
                }
            }

            return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    private async Task EvaluateIdeaByAverageScore(Idea idea, StageIdea stageIdea)
    {
        var totalCouncils = await _ideaVersionRequestRepository.CountCouncilsForIdea(idea.Id);
        var requiredReviewers = stageIdea.NumberReviewer ?? MIN_REVIEWERS;

        if (totalCouncils < requiredReviewers)
            return;

        var totalApproved = await _ideaVersionRequestRepository.CountApprovedCouncilsForIdea(idea.Id);
        var totalRejected = await _ideaVersionRequestRepository.CountRejectedCouncilsForIdea(idea.Id);
        var totalConsider = await _ideaVersionRequestRepository.CountConsiderCouncilsForIdea(idea.Id);

        var totalScore = (totalApproved * 1.0) + (totalConsider * 0.5);
        var averageScore = totalScore / totalCouncils;

        var status = averageScore switch
        {
            > APPROVAL_THRESHOLD => IdeaStatus.Approved,
            < APPROVAL_THRESHOLD => IdeaStatus.Rejected,
            _ => IdeaStatus.ConsiderByCouncil
        };

        await UpdateIdeaStatus(idea, stageIdea, status);
    }

    private async Task UpdateIdeaStatus(Idea idea, StageIdea stageIdeaCurrent, IdeaStatus status)
    {
        try
        {
            var ideaVersionCurrent = _ideaVersionRepository.GetQueryable()
                .Where(m => m.IdeaId == idea.Id &&
                            !m.IsDeleted)
                .OrderByDescending(m => m.Version)
                .FirstOrDefault();
            if (ideaVersionCurrent == null) return;

            if (status == IdeaStatus.Approved)
            {
                await HandleApprovedIdea(idea, ideaVersionCurrent, stageIdeaCurrent);
            }

            await UpdateIdeaCore(idea, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating idea {idea?.Id}");
            throw;
        }
    }

    private async Task HandleApprovedIdea(Idea idea, IdeaVersion ideaVersion, StageIdea stageIdea)
    {
        var ideaVersionsOfIdea = await _ideaVersionRepository.GetIdeaVersionsByIdeaId(ideaVersion.IdeaId.Value);
        var ideaVersionListId = ideaVersionsOfIdea.Select(m => m.Id).ToList().ConvertAll<Guid?>(x => x);
        var existingTopics = await _unitOfWork.TopicRepository.GetTopicByIdeaVersionId(ideaVersionListId);

        if (existingTopics.Count > 0)
        {
            var topic = existingTopics[0];
            topic.IdeaVersionId = ideaVersion.Id;
            _unitOfWork.TopicRepository.Update(topic);
            await _unitOfWork.SaveChanges();
        }

        // Kiểm tra xem IdeaVersion đã có Topic chưa
        var existingTopic = await _topicRepository.GetQueryable()
            .SingleOrDefaultAsync(m => m.IdeaVersionId == ideaVersion.Id);

        var existingTopicFilterds = existingTopics.Where(m => m.Id != existingTopic?.Id).ToList();
        if (existingTopicFilterds.Count != 0)
        {
            // remove
            _unitOfWork.TopicRepository.DeleteRangePermanently(existingTopicFilterds);
            await _unitOfWork.SaveChanges();
        }

        TopicResult? topicResult = null;

        if (existingTopic == null)
        {
            topicResult = await CreateTopicForIdea(ideaVersion, stageIdea);
            if (topicResult == null) return;
        }
        else
        {
            topicResult = new TopicResult
            {
                Id = existingTopic.Id,
                TopicCode = existingTopic.TopicCode
            };
        }

        // Mentor return
        var owner = await _unitOfWork.UserRepository.GetQueryable(m => m.Id == idea.OwnerId)
            .Include(e => e.UserXRoles).ThenInclude(e => e.Role).SingleOrDefaultAsync();
        var semesterGetUpComing = await _semesterRepository.GetUpComingSemester();

        // Nếu như kì sắp tới mà mentor chưa đc role student thì return
        if (owner?.UserXRoles?.Any(e => e.Role?.RoleName == "Mentor" && semesterGetUpComing?.Id == e.SemesterId) ==
            true)
            return;

        var existedProject = await _projectRepository.GetProjectByLeaderId(idea.OwnerId);

        if (existedProject == null)
        {
            await CreateNewProject(idea, ideaVersion, stageIdea, topicResult);
        }
        else
        {
            await UpdateExistingProject(existedProject, stageIdea, topicResult);
        }
    }

    public async Task<BusinessResult> GetIdeasOfReviewerByRolesAndStatus<TResult>(
        IdeaGetListByStatusAndRoleQuery query) where TResult : BaseResult
    {
        try
        {
            var userIdClaims = GetUserIdFromClaims();
            var userId = userIdClaims.Value;
            var (data, total) =
                await _ideaRepository.GetIdeasOfReviewerByRolesAndStatus(query,
                    userId);

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

    private async Task<TopicResult?> CreateTopicForIdea(IdeaVersion ideaVersion, StageIdea stageIdea)
    {
        try
        {
            var newTopicCode = await _semesterService.GenerateNewTopicCode(stageIdea.SemesterId);
            var codeExist = _topicRepository.IsExistedTopicCode(newTopicCode);
            if (codeExist) return null;

            var topicCreateCommand = new TopicCreateCommand
            {
                IdeaVersionId = ideaVersion.Id,
                TopicCode = newTopicCode
            };

            var res = await _topicService.CreateOrUpdate<TopicResult>(topicCreateCommand);

            return res.Status == 1 ? res.Data as TopicResult : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating topic for idea version {ideaVersion.Id}");
            return null;
        }
    }

    private async Task CreateNewProject(Idea idea, IdeaVersion ideaVersion, StageIdea stageIdea,
        TopicResult topicResult)
    {
        try
        {
            var newTeamCode = await _semesterService.GenerateNewTeamCode(stageIdea.SemesterId);
            var truncatedName = ideaVersion.EnglishName?.Length > MAX_TEAMNAME_LENGTH
                ? ideaVersion.EnglishName.Substring(0, MAX_TEAMNAME_LENGTH)
                : ideaVersion.EnglishName;

            var command = new ProjectCreateCommand
            {
                LeaderId = idea.OwnerId,
                TopicId = topicResult.Id,
                TeamCode = newTeamCode,
                TeamName = $"{truncatedName}-{Random.Shared.Next(RANDOM_SUFFIX_MIN, RANDOM_SUFFIX_MAX)}",
                TeamSize = 1,
                Status = ProjectStatus.Pending
            };

            await _projectService.CreateProjectAndTeammemberForAuto(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating new project for idea {idea.Id}");
            throw;
        }
    }

    private async Task UpdateExistingProject(Project existingProject, StageIdea stageIdea, TopicResult topicResult)
    {
        try
        {
            var newTeamCode = await _semesterService.GenerateNewTeamCode(stageIdea.SemesterId);
            existingProject.TopicId = topicResult.Id;
            existingProject.TeamCode = newTeamCode;
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

    private async Task UpdateIdeaCore(Idea idea, IdeaStatus status)
    {
        try
        {
            idea.Owner = null;
            idea.Mentor = null;
            idea.SubMentor = null;
            idea.Specialty = null;
            // idea.IdeaVersions = new List<IdeaVersion>();
            idea.Status = status;

            await SetBaseEntityForUpdate(idea);
            _ideaRepository.Update(idea);
            await _unitOfWork.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating core idea {idea.Id}");
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

            //get idea của kì với status khác approve => reject
            var ideas = await _ideaRepository.GetIdeaNotApproveInSemester(semester.Id);
            if (ideas != null)
            {
                foreach (var idea in ideas)
                {
                    idea.Status = IdeaStatus.Rejected;
                    await SetBaseEntityForUpdate(idea);
                }
                _ideaRepository.UpdateRange(ideas);

                //get idea version request (role mentor) của kì với status khác approve => reject
                var ideaVersionRequests = await _ideaVersionRequestRepository.GetRoleMentorNotApproveInSemester(semester.Id);
                if (ideaVersionRequests != null)
                {
                    foreach (var ideaVersionRequest in ideaVersionRequests)
                    {
                        ideaVersionRequest.Status = IdeaVersionRequestStatus.Rejected;
                        await SetBaseEntityForUpdate(ideaVersionRequest);
                    }
                    _ideaVersionRequestRepository.UpdateRange(ideaVersionRequests);
                }
            }

            //project k co topic => canceled
            var projectsWithNoTopic = await _projectRepository.GetPendingProjectsWithNoTopicStartingBySemesterId(semester.Id);
            if (projectsWithNoTopic != null)
            {
                foreach (var project in projectsWithNoTopic)
                {
                    project.Status = ProjectStatus.Canceled;
                    await SetBaseEntityForUpdate(project);
                }
                _projectRepository.UpdateRange(projectsWithNoTopic);
            }

            //get project của kì với status pending check xem idea approve có team size = với teamsize của nhóm k
            var projectsWithTopic = await _projectRepository.GetPendingProjectsWithTopicStartingBySemesterId(semester.Id);
            if (projectsWithTopic != null)
            {
                foreach (var project in projectsWithNoTopic)
                {
                    var ideaVersion = await _ideaVersionRepository.GetLastIdeaVersionByTopicId((Guid)project.TopicId);
                    if (ideaVersion != null)
                    {
                        //get team members
                        var members = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                        var numOfMembers = 0;
                        if (members != null)
                        {
                            numOfMembers = members.Count();
                        }

                        //teamsize != voi teamsize cua nhom, project status => canceled
                        if (ideaVersion.TeamSize != numOfMembers)
                        {
                            project.Status = ProjectStatus.Canceled;
                        }
                        //teamsize = voi teamsize cua nhom, project status => inprogress, teammember => inprogress
                        else
                        {
                            project.Status = ProjectStatus.InProgress;

                            foreach (var member in members)
                            {
                                member.Status = TeamMemberStatus.InProgress;
                                await SetBaseEntityForUpdate(member);
                            }
                            _teamMemberRepository.UpdateRange(members);
                        }
                        await SetBaseEntityForUpdate(project);
                    }
                }

                _projectRepository.UpdateRange(projectsWithTopic);
            }

            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
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
            var errorMessage = $"An error occurred in {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}