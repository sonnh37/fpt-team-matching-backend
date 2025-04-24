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
    private readonly ITopicRepository _topicRepository;
    private readonly IProjectService _projectService;
    private readonly ISemesterService _semesterService;
    private readonly ITopicService _topicService;
    private readonly ILogger<IdeaService> _logger;

    public IdeaService(IMapper mapper, IUnitOfWork unitOfWork,
        IProjectService projectService,
        ISemesterService semesterService,
        ITopicService topicService,
        INotificationService notificationService,
        ILogger<IdeaService> logger
    ) : base(mapper,
        unitOfWork)
    {
        _ideaRepository = unitOfWork.IdeaRepository;
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


    public async Task<BusinessResult> CreatePendingByStudent(IdeaStudentCreatePendingCommand ideaCreateModel)
    {
        try
        {
            //lay ra stageIdea hien tai
            var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
            if (stageIdea == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có đợt duyệt ứng với ngày hiện tại");
            }

            //ki cua stage idea
            var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có kì ứng với đợt duyệt hiện tại");
            }

            //check student co idea approve trong ki nay k
            var userId = GetUserIdFromClaims();
            var ideaApprovedInSemester =
                await _ideaRepository.GetIdeaApproveInSemesterOfUser((Guid)userId, semester.Id);
            if (ideaApprovedInSemester != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Sinh viên đã có đề tài được duyệt trong kì này");
            }

            //check student co idea dang pending trong dot duyet nay k
            var ideaPendingInStageIdea =
                await _ideaRepository.GetIdeaPendingInStageIdeaOfUser((Guid)userId, (Guid)stageIdea.Id);
            if (ideaPendingInStageIdea != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Sinh viên có đề tài đang trong quá trình duyệt ở kì này");
            }

            //var ideaEntity = _mapper.Map<Idea>(idea);
            //if (ideaEntity == null)
            //{
            //    return new ResponseBuilder()
            //        .WithStatus(Const.FAIL_CODE)
            //        .WithMessage(Const.FAIL_SAVE_MSG);
            //}

            //tao Idea
            var idea = new Idea
            {
                Id = Guid.NewGuid(),
                MentorId = ideaCreateModel.MentorId,
                SpecialtyId = ideaCreateModel.SpecialtyId,
                Status = IdeaStatus.Pending,
                OwnerId = userId,
                Type = IdeaType.Student,
                IsExistedTeam = true,
                IsEnterpriseTopic = false
            };
            await SetBaseEntityForCreation(idea);
            _ideaRepository.Add(idea);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            //tao IdeaVersion
            var ideaVersion = new IdeaVersion
            {
                Id = Guid.NewGuid(),
                IdeaId = idea.Id,
                StageIdeaId = stageIdea.Id,
                Version = 1,
                VietNamName = ideaCreateModel.VietNamName,
                EnglishName = ideaCreateModel.EnglishName,
                Description = ideaCreateModel.Description,
                Abbreviations = ideaCreateModel.Abbreviations,
                File = ideaCreateModel.File,
                TeamSize = ideaCreateModel.TeamSize,
            };

            await SetBaseEntityForCreation(ideaVersion);
            _ideaVersionRepository.Add(ideaVersion);
            saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            //tao IdeaVersionRequest cho mentor
            var ideaVersionRequest = new IdeaVersionRequest
            {
                IdeaVersionId = ideaVersion.Id,
                ReviewerId = idea.MentorId,
                CriteriaFormId = semester.CriteriaFormId,
                Status = IdeaVersionRequestStatus.Pending,
                Role = "Mentor",
            };
            await SetBaseEntityForCreation(ideaVersionRequest);
            _ideaVersionRequestRepository.Add(ideaVersionRequest);
            saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            //tao IdeaVersionRequest cho submentor (neu co)
            //var ideaVersionRequest = new IdeaVersionRequest
            //{
            //    IdeaVersionId = ideaVersion.Id,
            //    ReviewerId = idea.SubMentorId,
            //    CriteriaFormId = semester.CriteriaFormId,
            //    Status = IdeaVersionRequestStatus.Pending,
            //    Role = "SubMentor",
            //};

            #region cmt

            //ideaEntity.StageIdeaId = stageIdea.Id;
            //ideaEntity.Status = IdeaStatus.Pending;
            //ideaEntity.OwnerId = userId;
            //ideaEntity.Type = IdeaType.Student;
            //ideaEntity.IsExistedTeam = true;
            //ideaEntity.IsEnterpriseTopic = false;
            //await SetBaseEntityForCreation(ideaEntity);
            //_ideaRepository.Add(ideaEntity);

            //var saveChange = await _unitOfWork.SaveChanges();
            //if (!saveChange)
            //{
            //    return new ResponseBuilder()
            //        .WithStatus(Const.FAIL_CODE)
            //        .WithMessage(Const.FAIL_SAVE_MSG);
            //}

            //var ideaVersion = await CreateIdeaVersion(ideaEntity.Id, stageIdea.Id);

            // Tạo IdeaVersionRequest cho mentor
            //var mentorRequest = await CreateIdeaVersionRequest(
            //    ideaVersion.Id, 
            //    ideaEntity.MentorId, 
            //    "Mentor");

            // Nếu có submentor thì tạo request cho submentor
            //if (ideaEntity.SubMentorId != null)
            //{
            //    var subMentorRequest = await CreateIdeaVersionRequest(
            //        ideaVersion.Id, 
            //        ideaEntity.SubMentorId, 
            //        "SubMentor");
            //}

            #endregion

            //gửi noti cho mentor
            var command = new NotificationCreateForIndividual
            {
                UserId = idea.MentorId,
                Description = "Đề tài " + ideaVersion.Abbreviations + " đang chờ bạn duyệt với vai trò Mentor",
            };
            await _notificationService.CreateForUser(command);
            //gửi noti cho submentor (nếu có)

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating : {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    private async Task<IdeaVersion> CreateIdeaVersion(Guid ideaId, Guid stageIdeaId)
    {
        var ideaVersion = new IdeaVersion
        {
            IdeaId = ideaId,
            StageIdeaId = stageIdeaId,
            Version = 1
        };

        await SetBaseEntityForCreation(ideaVersion);
        _ideaVersionRepository.Add(ideaVersion);

        return ideaVersion;
    }

    private async Task<IdeaVersionRequest> CreateIdeaVersionRequest(
        Guid ideaVersionId,
        Guid? reviewerId,
        string role)
    {
        var ideaVersionRequest = new IdeaVersionRequest
        {
            IdeaVersionId = ideaVersionId,
            ReviewerId = reviewerId,
            Status = IdeaVersionRequestStatus.Pending,
            Role = role,
            ProcessDate = DateTime.UtcNow
        };

        await SetBaseEntityForCreation(ideaVersionRequest);
        _ideaVersionRequestRepository.Add(ideaVersionRequest);

        return ideaVersionRequest;
    }


    public async Task<BusinessResult> CreatePendingByLecturer(IdeaLecturerCreatePendingCommand ideaCreateModel)
    {
        try
        {
            //lay ra stageIdea hien tai
            var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
            if (stageIdea == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có đợt duyệt ứng với ngày hiện tại");
            }

            //ki cua stage idea
            var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có kì ứng với đợt duyệt hiện tại");
            }

            //check đề tài đki thứ 5 phải có submentor
            var userId = GetUserIdFromClaims();
            var numberOfIdeaMentorOrOwner = await _ideaRepository.NumberOfIdeaMentorOrOwner((Guid)userId);
            if (numberOfIdeaMentorOrOwner > 4)
            {
                //k co submentor
                if (ideaCreateModel.SubMentorId == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Đề tài thứ 5 trở lên cần có submentor");
                }

                //k tim thay submentor
                var submentor = await _userRepository.GetById((Guid)ideaCreateModel.SubMentorId);
                if (submentor == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Không tồn tại submentor");
                }
            }

            //check đề tài doanh nghiệp thì phải nhập tên doanh nghiệp
            if (ideaCreateModel.IsEnterpriseTopic)
            {
                if (ideaCreateModel.EnterpriseName == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Đề tài doanh nghiệp cần nhập tên doanh nghiệp");
                }
            }

            //tao Idea
            var idea = new Idea
            {
                Id = Guid.NewGuid(),
                MentorId = userId,
                SpecialtyId = ideaCreateModel.SpecialtyId,

                Status = IdeaStatus.Pending,
                OwnerId = userId,
                IsExistedTeam = false,
                IsEnterpriseTopic = ideaCreateModel.IsEnterpriseTopic,
            };
            if (idea.IsEnterpriseTopic)
            {
                idea.Type = IdeaType.Enterprise;
            }
            else
            {
                idea.Type = IdeaType.Lecturer;
            }

            await SetBaseEntityForCreation(idea);
            _ideaRepository.Add(idea);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            //tao IdeaVersion
            var ideaVersion = new IdeaVersion
            {
                Id = Guid.NewGuid(),
                IdeaId = idea.Id,
                StageIdeaId = stageIdea.Id,
                Version = 1,
                VietNamName = ideaCreateModel.VietNamName,
                EnglishName = ideaCreateModel.EnglishName,
                Description = ideaCreateModel.Description,
                Abbreviations = ideaCreateModel.Abbreviations,
                File = ideaCreateModel.File,
                TeamSize = ideaCreateModel.TeamSize,
            };
            if (idea.IsEnterpriseTopic)
            {
                ideaVersion.EnterpriseName = ideaCreateModel.EnterpriseName;
            }

            await SetBaseEntityForCreation(ideaVersion);
            _ideaVersionRepository.Add(ideaVersion);
            saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            //tao IdeaVersionRequest cho mentor
            var ideaVersionRequest = new IdeaVersionRequest
            {
                IdeaVersionId = ideaVersion.Id,
                ReviewerId = idea.MentorId,
                CriteriaFormId = semester.CriteriaFormId,
                Status = IdeaVersionRequestStatus.Approved,
                Role = "Mentor",
            };
            await SetBaseEntityForCreation(ideaVersionRequest);
            _ideaVersionRequestRepository.Add(ideaVersionRequest);

            saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            //tao IdeaVersionRequest cho submentor (neu co)
            //var ideaVersionRequest = new IdeaVersionRequest
            //{
            //    IdeaVersionId = ideaVersion.Id,
            //    ReviewerId = idea.SubMentorId,
            //    CriteriaFormId = semester.CriteriaFormId,
            //    Status = IdeaVersionRequestStatus.Pending,
            //    Role = "SubMentor",
            //};

            //gửi noti cho submentor (nếu có)
            //var command = new NotificationCreateCommand
            //{
            //    UserId = userId,
            //    Description = "Test, create idea",
            //    Type = NotificationType.General,
            //    IsRead = false,
            //};
            //await _notificationService.CreateOrUpdate<NotificationResult>(command);
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating : {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
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
            //sua db
            //if (project == null || project.Idea == null) return HandlerFail("Not found project");

            //ideaUpdateCommand.OwnerId = project.Idea.OwnerId;
            //ideaUpdateCommand.MentorId = project.Idea.MentorId;
            //ideaUpdateCommand.SubMentorId = project.Idea.SubMentorId;
            //ideaUpdateCommand.IdeaCode = project.Idea.IdeaCode;
            //ideaUpdateCommand.SpecialtyId = project.Idea.SpecialtyId;
            //ideaUpdateCommand.IsExistedTeam = project.Idea.IsExistedTeam;
            //ideaUpdateCommand.IsEnterpriseTopic = project.Idea.IsEnterpriseTopic;
            //ideaUpdateCommand.EnterpriseName = project.Idea.EnterpriseName;
            //ideaUpdateCommand.MaxTeamSize = project.Idea.MaxTeamSize;

            var command = _mapper.Map<Idea>(ideaUpdateCommand);
            //sua db
            //command.Id = project.Idea.Id;
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

    public async Task AutoUpdateIdeaStatus()
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AutoUpdateIdeaStatus");
            throw;
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
                .FirstOrDefault(m => m.IdeaId == idea.Id && m.StageIdeaId == stageIdeaCurrent.Id && !m.IsDeleted);

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
        if (idea.Owner?.UserXRoles?.Any(e => e.Role?.RoleName == "Student") != true)
            return;

        // Kiểm tra xem IdeaVersion đã có Topic chưa
        var existingTopic = await _topicRepository.GetQueryable().SingleOrDefaultAsync(m => m.IdeaVersionId == ideaVersion.Id);
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

        var existedProject = await _projectRepository.GetProjectByLeaderId(idea.OwnerId);

        if (existedProject == null)
        {
            await CreateNewProject(idea, ideaVersion, stageIdea, topicResult);
        }
        else
        {
            await UpdateExistingProject(existedProject, topicResult);
        }
    }

    private async Task<TopicResult?> CreateTopicForIdea(IdeaVersion ideaVersion, StageIdea stageIdea)
    {
        try
        {
            var newTopicCode = await _semesterService.GenerateNewTopicCode(stageIdea.SemesterId);
            var res = await _topicService.CreateOrUpdate<TopicResult>(new TopicCreateCommand
            {
                IdeaVersionId = ideaVersion.Id,
                TopicCode = newTopicCode,
            });

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

    private async Task UpdateExistingProject(Project existingProject, TopicResult topicResult)
    {
        try
        {
            existingProject.TopicId = topicResult.Id;
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

    public async Task AutoUpdateProjectInProgress()
    {
        try
        {
            //get projects cua ki den ngay bat dau inprogress - ngay khoa
            var projects = await _projectRepository.GetProjectsStartingNow();
            //update project
            foreach (var project in projects)
            {
                //sua db
                //var semester = project.Idea.StageIdea.Semester;
                //var semester = project.Topic.Idea.StageIdea.Semester;
                var semester = project.Topic.IdeaVersion.StageIdea.Semester;
                //update status
                project.Status = ProjectStatus.InProgress;
                //update teamCode
                //get so luong project InProgress cua ki
                var numberOfProjects = await _projectRepository.NumberOfInProgressProjectInSemester(semester.Id);
                // Tạo số thứ tự tiếp theo
                int nextNumber = numberOfProjects + 1;
                string semesterCode = semester.SemesterCode;
                //Tạo mã nhóm
                string newTeamCode = $"{semesterCode}SE{nextNumber:D3}";
                //sua db
                //project.Idea = null;
                project.Topic.IdeaVersion = null;
                await SetBaseEntityForUpdate(project);
                _projectRepository.Update(project);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return;
                }

                //update status cua teammember
                var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                foreach (var teamMember in teamMembers)
                {
                    await SetBaseEntityForUpdate(teamMember);
                    teamMember.Status = TeamMemberStatus.InProgress;
                }

                _teamMemberRepository.UpdateRange(teamMembers);
                isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
}