using AutoMapper;
using DocumentFormat.OpenXml.Math;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicRequests;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Services;

public class TopicRequestService : BaseService<TopicRequest>, ITopicRequestService
{
    private readonly ITopicRequestRepository _topicRequestRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IStageTopicRepository _stageTopicRepositoty;
    private readonly IUserRepository _userRepository;
    private readonly IAnswerCriteriaRepository _answerCriteriaRepository;
    private readonly INotificationService _notificationService;
    private readonly ISemesterService _semesterService;

    public TopicRequestService(IMapper mapper, IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ISemesterService semesterService
    ) : base(mapper,
        unitOfWork)
    {
        _semesterRepository = unitOfWork.SemesterRepository;
        _stageTopicRepositoty = unitOfWork.StageTopicRepository;
        _topicRepository = unitOfWork.TopicRepository;
        _userRepository = unitOfWork.UserRepository;
        _answerCriteriaRepository = unitOfWork.AnswerCriteriaRepository;
        _notificationService = notificationService;
        _semesterService = semesterService;
        _topicRequestRepository = unitOfWork.TopicRequestRepository;
    }

    public async Task CreateVersionRequests(Topic topic, Guid versionId, Guid criteriaFormId)
    {
        var semesterUpComing = await _semesterRepository.GetUpComingSemester();
        var mentor = await GetUserByRole("Mentor", semesterUpComing?.Id);
        // Mentor request
        var mentorRequest = new TopicRequest
        {
            ReviewerId = topic.MentorId,
            CriteriaFormId = criteriaFormId,
            Status = mentor != null ? TopicRequestStatus.Approved : TopicRequestStatus.Pending,
            Role = "Mentor",
        };
        await SetBaseEntityForCreation(mentorRequest);
        _topicRequestRepository.Add(mentorRequest);

        // Submentor request if exists
        // if (topic.SubMentorId.HasValue)
        // {
        //     var subMentorRequest = new TopicVersionRequest
        //     {
        //         TopicVersionId = versionId,
        //         ReviewerId = topic.SubMentorId.Value,
        //         CriteriaFormId = criteriaFormId,
        //         Status = TopicVersionRequestStatus.Pending,
        //         Role = "SubMentor",
        //     };
        //     await SetBaseEntityForCreation(subMentorRequest);
        //     _topicVersionRequestRepository.Add(subMentorRequest);
        // }
    }

    public async Task CreateVersionRequesForFirstCreateTopic(Topic topic, Guid versionId, Guid criteriaFormId)
    {
        var semesterUpComing = await _semesterRepository.GetUpComingSemester();
        var mentor = await GetUserByRole("Mentor", semesterUpComing?.Id);
        // Mentor request
        var mentorRequest = new TopicRequest
        {
            ReviewerId = topic.MentorId,
            CriteriaFormId = criteriaFormId,
            Status = mentor != null ? TopicRequestStatus.Approved : TopicRequestStatus.Pending,
            Role = "Mentor",
        };
        await SetBaseEntityForCreation(mentorRequest);
        _topicRequestRepository.Add(mentorRequest);

        // Submentor request if exists
        if (topic.SubMentorId.HasValue)
        {
            var subMentorRequest = new TopicRequest
            {
                ReviewerId = topic.SubMentorId.Value,
                CriteriaFormId = criteriaFormId,
                Status = TopicRequestStatus.Pending,
                Role = "SubMentor",
            };
            await SetBaseEntityForCreation(subMentorRequest);
            _topicRequestRepository.Add(subMentorRequest);
        }
    }

    public async Task<BusinessResult>
        GetAll<TResult>(TopicRequestGetAllQuery query) where TResult : BaseResult
    {
        try
        {
            var (data, total) = await _topicRequestRepository.GetData(query);

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
        GetAllExceptPending<TResult>(TopicRequestGetAllQuery query) where TResult : BaseResult
    {
        try
        {
            var (data, total) = await _topicRequestRepository.GetDataExceptPending(query);

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

    public async Task<BusinessResult> GetTopicRequestsForCurrentReviewerByRolesAndStatus<TResult>(
        TopicRequestGetListByStatusAndRoleQuery query) where TResult : BaseResult
    {
        try
        {
            var userIdClaims = GetUserIdFromClaims();
            if (userIdClaims == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy userId");
            }

            var userId = userIdClaims.Value;
            var (data, total) =
                await _topicRequestRepository.GetTopicVersionRequestsForCurrentReviewerByRolesAndStatus(query, userId);

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
            var (data, total) = await _topicRequestRepository.GetDataUnassignedReviewer(query);

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

    public async Task<BusinessResult> CreateCouncilRequestsForTopic(TopicRequestCreateForCouncilsCommand command)
    {
        try
        {
            //lay ra stageTopic hien tai
            var stageTopic = await _stageTopicRepositoty.GetCurrentStageTopic();
            if (stageTopic == null) return HandlerFail("Không có đợt duyệt ứng với ngày hiện tại");

            //ki cua stage topic
            var semester = await _semesterRepository.GetSemesterByStageTopicId(stageTopic.Id);
            if (semester == null) return HandlerFail("Không có kì ứng với đợt duyệt hiện tại");

            //var topicVersion = await _topicVersionRepository.GetById(command.TopicVersionId);
            //if (topicVersion == null) return HandlerFail("Ko tìm thấy topicVersionId");

            //if (command.TopicVersionId == Guid.Empty || command.TopicVersionId == null)
            //    return HandlerFail("Nhập topic version ");

            //var councils =
            //    await _userRepository.GetCouncilsForTopicVersionRequest(command.TopicVersionId.Value, semester.Id);
            //if (councils.Count == 0) return HandlerFail("Không có người dùng có thể tham gia hội đồng");

            var newTopicVersionRequests = new List<TopicRequest>();

            //foreach (var council in councils)
            //{
            //    var topicVersionRequest = new TopicRequest
            //    {
            //        TopicVersionId = command.TopicVersionId,
            //        CriteriaFormId = semester.CriteriaFormId,
            //        ReviewerId = council.Id,
            //        Status = TopicVersionRequestStatus.Pending,
            //        Role = "Council",
            //    };
            //    await SetBaseEntityForCreation(topicVersionRequest);
            //    newTopicVersionRequests.Add(topicVersionRequest);
            //}

            if (newTopicVersionRequests.Count == 0)
                return HandlerNotFound("Không có người dùng có thể tham gia hội đồng");

            _topicRequestRepository.AddRange(newTopicVersionRequests);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange) return HandlerFail("Đã xảy ra lỗi khi tạo yêu cầu đến hội đồng");

            //send noti cho councils
            //var request = new NotificationCreateForGroupUser
            //{
            //    Description = "Đề tài " + topicVersion?.Abbreviations + " đang chờ bạn duyệt với vai trò Council",
            //};
            //await _notificationService.CreateForGroupUsers(request, councils.Select(e => e.Id).ToList());

            //mentor nop topic -> topic 
            //tao topic
            //Gen topic code
            //var topicVersionsOfTopic = await _topicVersionRepository.GetTopicVersionsByTopicId(topicVersion.TopicId.Value);
            //var topicVersionListId = topicVersionsOfTopic.Select(m => m.Id).ToList().ConvertAll<Guid?>(x => x);
            //var existingTopics = await _unitOfWork.TopicRepository.GetTopicByTopicVersionId(topicVersionListId);

            //if (existingTopics.Count != 0)
            //    return new ResponseBuilder()
            //        .WithStatus(Const.SUCCESS_CODE)
            //        .WithMessage(Const.SUCCESS_READ_MSG);

            //var newTopicCode = await _semesterService.GenerateNewTopicCode(stageTopic.SemesterId);

            //var codeExist = _topicRepository.IsExistedTopicCode(newTopicCode);
            //if (codeExist) return HandlerFail("Trùng mã đề tài!");

            //var topicCreateCommand = new TopicCreateCommand
            //{
            //    TopicVersionId = topicVersion?.Id,
            //    TopicCode = newTopicCode
            //};

            //var res = await _topicService.CreateOrUpdate<TopicOldResult>(topicCreateCommand);

            return null;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> RespondByMentorOrManager(
        TopicRequestMentorOrManagerResponseCommand command)
    {
        try
        {
            var topicRequest = await _topicRequestRepository.GetById(command.Id);
            if (topicRequest == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage("Không tìm thấy topic request");
            }

            topicRequest.Status = command.Status;
            topicRequest.ProcessDate = DateTime.UtcNow;
            topicRequest.Note = command.Note;

            await SetBaseEntityForUpdate(topicRequest);
            _topicRequestRepository.Update(topicRequest);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            var topicRequestPendings = await _topicRequestRepository.GetQueryable(m =>
                m.TopicId == topicRequest.TopicId && m.Status == TopicRequestStatus.Pending).ToListAsync();

            var topic = await _topicRepository.GetById(topicRequest.TopicId);
            if (topic == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
            }

            //neu mentor response 
            if (topicRequest.Role == "Mentor" || topicRequest.Role == "SubMentor")
            {
                //neu la status la consider -> sua status cua topic -> MentorConsidered
                if (topicRequest.Status == TopicRequestStatus.Consider)
                {
                    topic.Status = TopicStatus.MentorConsider;
                    await SetBaseEntityForUpdate(topic);
                    _topicRepository.Update(topic);
                    if (!await _unitOfWork.SaveChanges())
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                //neu la status la reject -> sua status cua topic -> reject
                if (topicRequest.Status == TopicRequestStatus.Rejected)
                {
                    topic.Status = TopicStatus.MentorRejected;
                    await SetBaseEntityForUpdate(topic);
                    _topicRepository.Update(topic);
                    if (!await _unitOfWork.SaveChanges())
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                //neu la status la reject -> sua status cua topic -> reject
                if (topicRequest.Status == TopicRequestStatus.Approved && topicRequestPendings.Count == 0)
                {
                    topic.Status = TopicStatus.MentorApproved;
                    await SetBaseEntityForUpdate(topic);
                    _topicRepository.Update(topic);
                    if (!await _unitOfWork.SaveChanges())
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                //noti cho owner
                var topicInclude = await _topicRepository.GetById(topicRequest.TopicId, true);
                var noti = new NotificationCreateForIndividual
                {
                    UserId = topicInclude?.OwnerId,
                };

                if (topicRequest.Role == "Mentor" && topicRequestPendings.Count == 0)
                {
                    noti.Description = "Đề tài " + topicInclude?.Abbreviation + " đã được " +
                                       topicInclude?.Mentor?.Code + " (Mentor) duyệt. Hãy kiểm tra kết quả!";
                    await _notificationService.CreateForUser(noti);
                }

                if (topicRequest.Role == "SubMentor" && topicRequestPendings.Count == 0)
                {
                    noti.Description = "Đề tài " + topicRequest.Topic?.Abbreviation + " đã được " +
                                       topicInclude?.SubMentor?.Code + " (SubMentor) duyệt. Hãy kiểm tra kết quả!";
                    await _notificationService.CreateForUser(noti);
                }
            }

            //neu manager reponse
            if (topicRequest.Role == "Manager")
            {
                //neu la status la reject -> sua status cua topic -> reject
                if (topicRequest.Status == TopicRequestStatus.Rejected)
                {
                    topic.Status = TopicStatus.ManagerRejected;
                    await SetBaseEntityForUpdate(topic);
                    _topicRepository.Update(topic);
                    if (!await _unitOfWork.SaveChanges())
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }

                if (topicRequest.Status == TopicRequestStatus.Approved)
                {
                    topic.Status = TopicStatus.ManagerApproved;
                    await SetBaseEntityForUpdate(topic);
                    _topicRepository.Update(topic);
                    if (!await _unitOfWork.SaveChanges()) return HandlerFail(Const.FAIL_SAVE_MSG);
                }
            }


            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TopicRequestResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}