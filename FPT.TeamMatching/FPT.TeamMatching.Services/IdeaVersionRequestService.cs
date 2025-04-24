using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using NetTopologySuite.Triangulate.Tri;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

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
    private readonly IIdeaService _ideaService;
    private readonly IAnswerCriteriaRepository _answerCriteriaRepository;
    private readonly INotificationService _notificationService;

    public IdeaVersionRequestService(IMapper mapper, IUnitOfWork unitOfWork, IIdeaService ideaService,
        INotificationService notificationService) : base(mapper,
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
        _ideaService = ideaService;
        _notificationService = notificationService;
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
        IdeaVersionRequestGetListByStatusAndRoleQuery query) where TResult : BaseResult
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

            if (command.IdeaVersionId == Guid.Empty || command.IdeaVersionId == null)
                return HandlerFail("Nhập idea version ");
            var councils = await _userRepository.GetCouncilsForIdeaVersionRequest(command.IdeaVersionId.Value);
            if (!councils.Any()) return HandlerFail("No available councils");

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

            if (!newIdeaVersionRequests.Any()) return HandlerNotFound("No available councils");

            _ideaVersionRequestRepository.AddRange(newIdeaVersionRequests);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Failed to create council requests.");
            }

            var ideaVersion = await _ideaVersionRepository.GetById((Guid)command.IdeaVersionId);
            //send noti cho 3 nguoi council
            var request = new NotificationCreateForGroupUser
            {
                Description = "Đề tài " + ideaVersion.Abbreviations + " đang chờ bạn duyệt với vai trò Council",
            };
            await _notificationService.CreateForGroupUsers(request, councils.Select(e => e.Id).ToList());
            //


            //mentor nop idea -> topic 
            //tao topic
            //Gen topic code
            var semesterCode = semester.SemesterCode;
            var semesterPrefix = semester.SemesterPrefixName;
            //get so luong topic cua ki
            var numberOfTopic = _topicRepository.NumberOfTopicBySemesterId(semester.Id);

            // Tạo số thứ tự tiếp theo
            int nextNumberTopic = numberOfTopic + 1;

            // Tạo topic code mới theo định dạng: semesterPrefix + semesterCode + "SE" + số thứ tự (3 chữ số)
            string newTopicCode = $"{semesterPrefix}{semesterCode}SE{nextNumberTopic:D3}";

            var topic = new Topic
            {
                IdeaVersionId = ideaVersion.Id,
            };
            //check k trùng topic code
            var codeExist = _topicRepository.IsExistedTopicCode(newTopicCode);
            if (!codeExist)
            {
                topic.TopicCode = newTopicCode;
            }

            await SetBaseEntityForCreation(topic);
            _topicRepository.Add(topic);

            saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> UpdateStatus(IdeaVersionRequestUpdateStatusCommand command)
    {
        try
        {
            var ideaVersionRequest = await _ideaVersionRequestRepository.GetById(command.Id);
            if (ideaVersionRequest == null) return HandlerFail("Not found ideaVersionRequest");

            ideaVersionRequest.Status = command.Status;
            //sua db
            //ideaVersionRequest.Content = command.Content;
            ideaVersionRequest.ProcessDate = DateTime.UtcNow;
            _ideaVersionRequestRepository.Update(ideaVersionRequest);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            // Nếu Mentor từ chối -> Idea bị từ chối ngay lập tức
            if (command.Status == IdeaVersionRequestStatus.Rejected && ideaVersionRequest.Role == "Mentor")
            {
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                {
                    Id = ideaVersionRequest.IdeaVersionId.Value,
                    Status = IdeaStatus.Rejected
                });
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> ProcessCouncilDecision(Guid ideaId)
    {
        var totalCouncils = await _ideaVersionRequestRepository.CountCouncilsForIdea(ideaId);
        var totalApproved = await _ideaVersionRequestRepository.CountApprovedCouncilsForIdea(ideaId);
        var totalRejected = await _ideaVersionRequestRepository.CountRejectedCouncilsForIdea(ideaId);

        if (totalCouncils == 1)
        {
            if (totalApproved == 1)
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                    { Id = ideaId, Status = IdeaStatus.Approved });

            if (totalRejected == 1)
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                    { Id = ideaId, Status = IdeaStatus.Rejected });
        }
        else if (totalCouncils == 2)
        {
            if (totalApproved == 2)
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                    { Id = ideaId, Status = IdeaStatus.Approved });

            if (totalRejected == 2)
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                    { Id = ideaId, Status = IdeaStatus.Rejected });
        }
        else if (totalCouncils == 3)
        {
            if (totalApproved > totalRejected)
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                    { Id = ideaId, Status = IdeaStatus.Approved });

            if (totalRejected > totalApproved)
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                    { Id = ideaId, Status = IdeaStatus.Rejected });
        }

        return new ResponseBuilder().WithStatus(Const.SUCCESS_CODE).WithMessage(Const.SUCCESS_SAVE_MSG);
    }

    //public async Task<BusinessResult> CouncilResponse(IdeaVersionRequestLecturerOrCouncilResponseCommand command)
    //{
    //    try
    //    {
    //        var user = await GetUserAsync();
    //        var ideaVersionRequest = _mapper.Map<IdeaVersionRequest>(command);
    //        var ideaVersionRequestOld = await _ideaVersionRequestRepository.GetById(ideaVersionRequest.Id);
    //        if (ideaVersionRequestOld != null)
    //        {
    //            ideaVersionRequestOld.Status = ideaVersionRequest.Status;
    //            ideaVersionRequestOld.Content = ideaVersionRequest.Content;
    //            ideaVersionRequestOld.ProcessDate = DateTime.UtcNow;
    //            ideaVersionRequestOld.ReviewerId = user.Id;
    //            await SetBaseEntityForUpdate(ideaVersionRequestOld);
    //            _ideaVersionRequestRepository.Update(ideaVersionRequestOld);

    //            var idea = await _ideaRepository.GetById(ideaVersionRequestOld.IdeaId.Value);


    //                 ////Gen idea code 
    //                 //var semester = await _semesterRepository.GetById((Guid)idea.SemesterId);
    //                 //var semesterCode = semester.SemesterCode;

    //                 ////lấy số thứ tự đề tài lớn nhất của kì học 
    //                 //var maxNumber = await _ideaRepository.MaxNumberOfSemester((Guid)idea.SemesterId);

    //                 //// Tạo số thứ tự tiếp theo
    //                 //int nextNumber = maxNumber + 1;

    //                 //// Tạo mã Idea mới theo định dạng: semesterCode + "SE" + số thứ tự (2 chữ số)
    //                 //string newIdeaCode = $"{semesterCode}SE{nextNumber:D2}";

    //                 //idea.IdeaCode = newIdeaCode;
    //             }

    //            await SetBaseEntityForUpdate(idea);
    //            _ideaRepository.Update(idea);

    //            bool saveChange = await _unitOfWork.SaveChanges();
    //            if (saveChange)
    //            {
    //                return new ResponseBuilder()
    //                    .WithStatus(Const.SUCCESS_CODE)
    //                    .WithMessage(Const.SUCCESS_SAVE_MSG);
    //            }

    //            return new ResponseBuilder()
    //                .WithStatus(Const.FAIL_CODE)
    //                .WithMessage(Const.FAIL_SAVE_MSG);
    //        }

    //        return new ResponseBuilder()
    //            .WithStatus(Const.NOT_FOUND_CODE)
    //            .WithMessage("Not found idea");
    //    }
    //    catch (Exception ex)
    //    {
    //        var errorMessage = $"An error {typeof(IdeaVersionRequestResult).Name}: {ex.Message}";
    //        return new ResponseBuilder()
    //            .WithStatus(Const.FAIL_CODE)
    //            .WithMessage(errorMessage);
    //    }
    //}

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

            await SetBaseEntityForUpdate(ideaVersionRequest);
            _ideaVersionRequestRepository.Update(ideaVersionRequest);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            var answerCriteriaList = _mapper.Map<List<AnswerCriteria>>(command.AnswerCriteriaList);
            foreach (var answerCriteria in answerCriteriaList)
            {
                answerCriteria.IdeaVersionRequestId = ideaVersionRequest.Id;
                await SetBaseEntityForCreation(answerCriteria);
            }

            _answerCriteriaRepository.AddRange(answerCriteriaList);
            saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
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
            if (ideaVersionRequest.Role == "Mentor")
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