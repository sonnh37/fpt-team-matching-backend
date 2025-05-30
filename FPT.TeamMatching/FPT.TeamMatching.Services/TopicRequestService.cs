using AutoMapper;
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
            if (semester == null) return HandlerFail("Không có kỳ ứng với đợt duyệt hiện tại");

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
        TopicRequestForRespondCommand command)
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

            var topic = await _topicRepository.GetById(topicRequest.TopicId);
            if (topic == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
            }

            var topicInclude = await _topicRepository.GetById(topicRequest.TopicId, true);
            var noti = new NotificationCreateForIndividual
            {
                UserId = topicInclude?.OwnerId,
            };

            //neu mentor response 
            if (topicRequest.Role == "Mentor")
            {
                //neu la status la reject -> sua status cua topic -> reject
                if (topicRequest.Status == TopicRequestStatus.Rejected)
                {
                    topic.Status = TopicStatus.MentorRejected;
                }

                //neu la status la approve -> sua status cua topic -> approve
                if (topicRequest.Status == TopicRequestStatus.Approved)
                {
                    var approveTopicRequestOfSubMentor = await _topicRequestRepository.GetByTopicIdAndRoleAndStatus(
                        topic.Id, "SubMentor", TopicRequestStatus.Approved);
                    var shouldApprove = topic.SubMentorId == null ||
                                        (topic.SubMentorId != null &&
                                         approveTopicRequestOfSubMentor.Any());

                    if (shouldApprove)
                    {
                        topic.Status = TopicStatus.MentorApproved;
                    }
                }

                //neu la status la consider -> sua status cua topic -> MentorConsidered
                if (topicRequest.Status == TopicRequestStatus.Consider)
                {
                    var approveTopicRequestOfSubMentor =
                        await _topicRequestRepository.GetByTopicIdAndRoleAndStatus(topic.Id, "SubMentor",
                            TopicRequestStatus.Approved);

                    var shouldApprove = topic.SubMentorId == null ||
                                        (topic.SubMentorId != null &&
                                         approveTopicRequestOfSubMentor.Any());

                    if (shouldApprove)
                    {
                        topic.Status = TopicStatus.MentorConsider;
                    }
                }

                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return HandlerFail(Const.FAIL_SAVE_MSG);
                }

                //noti cho owner
                noti.Description = "Đề tài " + topicInclude?.Abbreviation + " đã được " +
                                   topicInclude?.Mentor?.Code + " (Mentor) duyệt. Hãy kiểm tra kết quả!";
                await _notificationService.CreateForUser(noti);
            }
            //neu submentor response
            else if (topicRequest.Role == "SubMentor")
            {
                //neu la status la reject -> sua status cua topic -> reject
                if (topicRequest.Status == TopicRequestStatus.Rejected)
                {
                    topic.Status = TopicStatus.MentorRejected;
                }

                //neu la status la approve -> sua status cua topic -> approve
                if (topicRequest.Status == TopicRequestStatus.Approved)
                {
                    var approveTopicRequestOfMentor =
                        await _topicRequestRepository.GetByTopicIdAndRoleAndStatus(topic.Id, "Mentor",
                            TopicRequestStatus.Approved);
                    var considerTopicRequestOfMentor =
                        await _topicRequestRepository.GetByTopicIdAndRoleAndStatus(topic.Id, "Mentor",
                            TopicRequestStatus.Consider);
                    var topicRequestOfMentor = await _topicRequestRepository.GetByTopicIdAndRole(topic.Id, "Mentor");

                    var shouldApprove = topicRequestOfMentor.Count > 0
                        ? (topic.SubMentorId == null ||
                           (topic.SubMentorId != null &&
                            approveTopicRequestOfMentor.Any()))
                        : (topic.SubMentorId != null);

                    if (shouldApprove)
                    {
                        topic.Status = TopicStatus.MentorApproved;
                    }

                    if (topicRequestOfMentor.Count > 0)
                    {
                        var shouldConsider = topic.SubMentorId == null ||
                                             (topic.SubMentorId != null &&
                                              considerTopicRequestOfMentor.Any());

                        if (shouldConsider)
                        {
                            topic.Status = TopicStatus.MentorConsider;
                        }
                    }
                    
                    
                }

                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess) return HandlerFail(Const.FAIL_SAVE_MSG);
                noti.Description = "Đề tài " + topicRequest.Topic?.Abbreviation + " đã được " +
                                   topicInclude?.SubMentor?.Code + " (SubMentor) duyệt. Hãy kiểm tra kết quả!";
                await _notificationService.CreateForUser(noti);
            }

            //neu manager reponse
            else if (topicRequest.Role == "Manager")
            {
                //neu la status la reject -> sua status cua topic -> reject
                if (topicRequest.Status == TopicRequestStatus.Rejected)
                {
                    topic.Status = TopicStatus.ManagerRejected;
                }

                //neu la status la approve -> sua status cua topic -> approve
                if (topicRequest.Status == TopicRequestStatus.Approved)
                {
                    topic.Status = TopicStatus.ManagerApproved;
                }

                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess) return HandlerFail(Const.FAIL_SAVE_MSG);

                //noti cho owner
                noti.Description = "Đề tài " + topicRequest.Topic?.Abbreviation +
                                   " đã được duyệt xong. Hãy kiểm tra kết quả!";
                await _notificationService.CreateForUser(noti);
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

    public async Task<BusinessResult> SendRequestToSubMentorByMentor(TopicRequestForSubMentorCommand command)
    {
        try
        {
            //semester is preparing
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return HandlerFail("Không tìm thấy kỳ");
            }

            if (semester.Status != SemesterStatus.Preparing)
            {
                return HandlerFail("Hiện tại không được gửi yêu cầu");
            }

            // check topic
            var topic = await _topicRepository.GetById(command.TopicId);
            if (topic == null)
            {
                return HandlerFail("Không tìm thấy đề tài");
            }

            //check topic co submentor chuaw
            if (topic.SubMentorId != null)
            {
                return HandlerFail("Đề tài đã có Mentor 2");
            }

            //check co role mentor trong ki nay k
            var submentor = await _userRepository.GetById(command.ReviewerId);
            if (submentor == null)
            {
                return HandlerFail("Không tìm thấy người dùng");
            }

            var isMentor = await _userRepository.CheckRoleOfUserInSemester(submentor.Id, "Mentor", semester.Id);
            if (!isMentor)
            {
                return HandlerFail("Người dùng không phải là Mentor ở học kỳ này");
            }

            //check con slot lam submentor k
            var topicBeSubMentor =
                await _topicRepository.GetTopicsBeSubMentorOfUserInSemester(submentor.Id, semester.Id);
            if (topicBeSubMentor.Count() == semester.LimitTopicSubMentor)
            {
                return HandlerFail("Mentor này đã làm Mentor 2 đủ số lượng nhóm quy định của kỳ: " +
                                   topicBeSubMentor.Count());
            }

            var topicRequest = new TopicRequest
            {
                TopicId = command.TopicId,
                ReviewerId = command.ReviewerId,
                Role = "SubMentor",
                Status = TopicRequestStatus.Pending
            };

            await SetBaseEntityForCreation(topicRequest);
            _topicRequestRepository.Add(topicRequest);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return HandlerFail("Đã xảy ra lỗi khi gửi yêu cầu");
            }

            //noti cho mentor - người gửi
            var user = await GetUserAsync();

            var noti = new NotificationCreateForIndividual
            {
                UserId = topicRequest.ReviewerId,
                Description = user?.Code + " đã gửi lời mời làm Mentor 2 cho đề tài " + topic.Abbreviation + " đến bạn"
            };
            await _notificationService.CreateForIndividual(noti);

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Gửi yêu cầu thành công");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TopicResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> SubMentorResponseRequestOfMentor(TopicRequestForRespondCommand command)
    {
        try
        {
            // check topic request
            var topicRequest = await _topicRequestRepository.GetById(command.Id);
            if (topicRequest == null)
            {
                return HandlerFail("Không tìm thấy yêu cầu");
            }

            //semester is preparing
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return HandlerFail("Không tìm thấy kỳ");
            }

            if (semester.Status != SemesterStatus.Preparing)
            {
                return HandlerFail("Hiện tại không được phản hồi");
            }

            var user = await GetUserAsync();

            var topic = await _topicRepository.GetById(topicRequest.TopicId);
            if (topic == null)
            {
                return HandlerFail("Không tìm thấy đề tài");
            }

            //neu approve -> them submentor vao topic
            if (command.Status == TopicRequestStatus.Approved)
            {
                //check con slot lam submentor k
                var topicBeSubMentor =
                    await _topicRepository.GetTopicsBeSubMentorOfUserInSemester(user.Id, semester.Id);
                if (topicBeSubMentor.Count() == semester.LimitTopicSubMentor)
                {
                    return HandlerFail("Bạn đã làm Mentor 2 đủ số lượng nhóm quy định của kì: " +
                                       topicBeSubMentor.Count());
                }

                topic.SubMentorId = user.Id;
                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
            }

            topicRequest.Status = command.Status;
            await SetBaseEntityForUpdate(topicRequest);
            _topicRequestRepository.Update(topicRequest);

            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return HandlerFail("Đã xảy ra lỗi khi phản hồi");
            }

            //noti cho mentor - người gửi
            var noti = new NotificationCreateForIndividual
            {
                UserId = topic.MentorId,
                Description = user?.Code + " đã phản hồi lời mời làm Mentor 2 cho đề tài " + topic.Abbreviation +
                              " của bạn"
            };
            await _notificationService.CreateForIndividual(noti);

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Phản hồi thành công");
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