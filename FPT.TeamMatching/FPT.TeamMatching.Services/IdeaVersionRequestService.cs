using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class IdeaVersionRequestService : BaseService<IdeaVersionRequest>, IIdeaVersionRequestService
{
    private readonly IIdeaVersionRequestRepository _ideaVersionRequestRepository;
    private readonly IIdeaVersionRepository _ideaVersionRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IStageIdeaRepositoty _stageIdeaRepositoty;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAnswerCriteriaRepository _answerCriteriaRepository;
    private readonly INotificationService _notificationService;
    private readonly ITopicService _topicService;
    private readonly ISemesterService _semesterService;

    public IdeaVersionRequestService(IMapper mapper, IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ITopicService topicService,
        ISemesterService semesterService
    ) : base(mapper,
        unitOfWork)
    {
        _ideaVersionRequestRepository = unitOfWork.IdeaVersionRequestRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _ideaVersionRepository = unitOfWork.IdeaVersionRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _stageIdeaRepositoty = unitOfWork.StageIdeaRepository;
        _topicRepository = unitOfWork.TopicRepository;
        _userRepository = unitOfWork.UserRepository;
        _answerCriteriaRepository = unitOfWork.AnswerCriteriaRepository;
        _notificationService = notificationService;
        _topicService = topicService;
        _semesterService = semesterService;
    }

    public async Task CreateVersionRequests(Idea idea, Guid versionId, Guid criteriaFormId)
    {
        var semesterUpComing = await _semesterRepository.GetUpComingSemester();
        var mentor = await GetUserByRole("Mentor", semesterUpComing?.Id);
        // Mentor request
        var mentorRequest = new IdeaVersionRequest
        {
            IdeaVersionId = versionId,
            ReviewerId = idea.MentorId,
            CriteriaFormId = criteriaFormId,
            Status = mentor != null ? IdeaVersionRequestStatus.Approved : IdeaVersionRequestStatus.Pending,
            Role = "Mentor",
        };
        await SetBaseEntityForCreation(mentorRequest);
        _ideaVersionRequestRepository.Add(mentorRequest);

        // Submentor request if exists
        if (idea.SubMentorId.HasValue)
        {
            var subMentorRequest = new IdeaVersionRequest
            {
                IdeaVersionId = versionId,
                ReviewerId = idea.SubMentorId.Value,
                CriteriaFormId = criteriaFormId,
                Status = IdeaVersionRequestStatus.Pending,
                Role = "SubMentor",
            };
            await SetBaseEntityForCreation(subMentorRequest);
            _ideaVersionRequestRepository.Add(subMentorRequest);
        }
    }

    public async Task<BusinessResult>
        GetAll<TResult>(IdeaVersionRequestGetAllQuery query) where TResult : BaseResult
    {
        try
        {
            var (data, total) = await _ideaVersionRequestRepository.GetData(query);

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

    public async Task<BusinessResult> GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus<TResult>(
        IdeaGetListByStatusAndRoleQuery query) where TResult : BaseResult
    {
        try
        {
            var userIdClaims = GetUserIdFromClaims();
            var userId = userIdClaims.Value;
            var (data, total) =
                await _ideaVersionRequestRepository.GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus(query,
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

    public async Task<BusinessResult>
        GetAllUnassignedReviewer<TResult>(GetQueryableQuery query) where TResult : BaseResult
    {
        try
        {
            var (data, total) = await _ideaVersionRequestRepository.GetDataUnassignedReviewer(query);

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

    public async Task<BusinessResult> CreateCouncilRequestsForIdea(IdeaVersionRequestCreateForCouncilsCommand command)
    {
        try
        {
            //lay ra stageIdea hien tai
            var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
            if (stageIdea == null) return HandlerFail("Không có đợt duyệt ứng với ngày hiện tại");

            //ki cua stage idea
            var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
            if (semester == null) return HandlerFail("Không có kì ứng với đợt duyệt hiện tại");

            var ideaVersion = await _ideaVersionRepository.GetById(command.IdeaVersionId);
            if (ideaVersion == null) return HandlerFail("Ko tìm thấy ideaVersionId");

            if (command.IdeaVersionId == Guid.Empty || command.IdeaVersionId == null)
                return HandlerFail("Nhập idea version ");

            var councils =
                await _userRepository.GetCouncilsForIdeaVersionRequest(command.IdeaVersionId.Value, semester.Id);
            if (councils.Count == 0) return HandlerFail("Không có người dùng có thể tham gia hội đồng");

            var newIdeaVersionRequests = new List<IdeaVersionRequest>();

            foreach (var council in councils)
            {
                var ideaVersionRequest = new IdeaVersionRequest
                {
                    IdeaVersionId = command.IdeaVersionId,
                    CriteriaFormId = semester.CriteriaFormId,
                    ReviewerId = council.Id,
                    Status = IdeaVersionRequestStatus.Pending,
                    Role = "Council",
                };
                await SetBaseEntityForCreation(ideaVersionRequest);
                newIdeaVersionRequests.Add(ideaVersionRequest);
            }

            if (newIdeaVersionRequests.Count == 0) return HandlerNotFound("Không có người dùng có thể tham gia hội đồng");

            _ideaVersionRequestRepository.AddRange(newIdeaVersionRequests);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange) return HandlerFail("Đã xảy ra lỗi khi tạo yêu cầu đến hội đồng");

            //send noti cho councils
            var request = new NotificationCreateForGroupUser
            {
                Description = "Đề tài " + ideaVersion?.Abbreviations + " đang chờ bạn duyệt với vai trò Council",
            };
            await _notificationService.CreateForGroupUsers(request, councils.Select(e => e.Id).ToList());

            //mentor nop idea -> topic 
            //tao topic
            //Gen topic code
            var ideaVersionsOfIdea = await _ideaVersionRepository.GetIdeaVersionsByIdeaId(ideaVersion.IdeaId.Value);
            var ideaVersionListId = ideaVersionsOfIdea.Select(m => m.Id).ToList().ConvertAll<Guid?>(x => x);
            var existingTopics = await _unitOfWork.TopicRepository.GetTopicByIdeaVersionId(ideaVersionListId);

            if (existingTopics.Count != 0)
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);

            var newTopicCode = await _semesterService.GenerateNewTopicCode(stageIdea.SemesterId);
            var codeExist = _topicRepository.IsExistedTopicCode(newTopicCode);
            if (codeExist) return HandlerFail("Trùng topic code");

            var topicCreateCommand = new TopicCreateCommand
            {
                IdeaVersionId = ideaVersion?.Id,
                TopicCode = newTopicCode
            };

            var res = await _topicService.CreateOrUpdate<TopicResult>(topicCreateCommand);

            return res;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> RespondByMentorOrCouncil(
        IdeaVersionRequestLecturerOrCouncilResponseCommand command)
    {
        try
        {
            var ideaVersionRequest = await _ideaVersionRequestRepository.GetById(command.Id, true);
            if (ideaVersionRequest == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage("Không tìm thấy idea version request");
            }

            ideaVersionRequest.Status = command.Status;
            ideaVersionRequest.ProcessDate = DateTime.UtcNow;
            ideaVersionRequest.Note = command.Note;

            await SetBaseEntityForUpdate(ideaVersionRequest);
            _ideaVersionRequestRepository.Update(ideaVersionRequest);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            if (ideaVersionRequest.Role != "SubMentor")
            {
                var answerCriteriaList = _mapper.Map<List<AnswerCriteria>>(command.AnswerCriteriaList);
                foreach (var answerCriteria in answerCriteriaList)
                {
                    answerCriteria.IdeaVersionRequestId = ideaVersionRequest.Id;
                    await SetBaseEntityForCreation(answerCriteria);
                }

                _answerCriteriaRepository.AddRange(answerCriteriaList);
                if (!await _unitOfWork.SaveChanges())
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }
            }
            

            if (ideaVersionRequest.IdeaVersion == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
            }

            if (ideaVersionRequest.IdeaVersion.IdeaId == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
            }

            //neu mentor response 
            if (ideaVersionRequest.Role == "Mentor" || ideaVersionRequest.Role == "SubMentor")
            {
                var idea = await _ideaRepository.GetById((Guid)ideaVersionRequest.IdeaVersion.IdeaId);
                if (idea == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);
                }

                //neu la status la consider -> sua status cua idea -> ConsiderByMentor
                if (ideaVersionRequest.Status == IdeaVersionRequestStatus.Consider)
                {
                    idea.Status = IdeaStatus.ConsiderByMentor;
                    await SetBaseEntityForUpdate(idea);
                    _ideaRepository.Update(idea);
                    saveChange = await _unitOfWork.SaveChanges();
                    if (!saveChange)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                //neu la status la reject -> sua status cua idea -> reject
                if (ideaVersionRequest.Status == IdeaVersionRequestStatus.Rejected)
                {
                    idea.Status = IdeaStatus.Rejected;
                    await SetBaseEntityForUpdate(idea);
                    _ideaRepository.Update(idea);
                    saveChange = await _unitOfWork.SaveChanges();
                    if (!saveChange)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                //noti cho owner
                var ideaInclude = await _ideaRepository.GetById((Guid)ideaVersionRequest.IdeaVersion.IdeaId, true);
                var noti = new NotificationCreateForIndividual
                {
                    UserId = ideaInclude?.OwnerId,
                    Description = "Đề tài " + ideaVersionRequest.IdeaVersion.Abbreviations + " đã được " +
                                  ideaInclude?.Mentor?.Code + " (Mentor) duyệt. Hãy kiểm tra kết quả!",
                };
                await _notificationService.CreateForUser(noti);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaVersionRequestResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}