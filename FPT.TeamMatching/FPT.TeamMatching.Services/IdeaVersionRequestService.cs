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
    private readonly IIdeaRepository _ideaRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIdeaService _ideaService;
    private readonly INotificationService _notificationService;

    public IdeaVersionRequestService(IMapper mapper, IUnitOfWork unitOfWork, IIdeaService ideaService, INotificationService notificationService) : base(mapper,
        unitOfWork)
    {
        _ideaVersionRequestRepository = unitOfWork.IdeaVersionRequestRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _userRepository = unitOfWork.UserRepository;
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

    public async Task<BusinessResult> GetIdeaRequestsForCurrentReviewerByRolesAndStatus<TResult>(
        IdeaVersionRequestGetListByStatusAndRoleQuery query) where TResult : BaseResult
    {
        try
        {
            var userIdClaims = GetUserIdFromClaims();
            var userId = userIdClaims.Value;
            var (data, total) = await _ideaVersionRequestRepository.GetIdeaRequestsForCurrentReviewerByRolesAndStatus(query, userId);

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
            //sua db
            //if (command.IdeaId == Guid.Empty || command.IdeaId == null) return HandlerFail("No Idea Id provided");
            //var councils = await _userRepository.GetThreeCouncilsForIdeaRequest(command.IdeaId.Value);
            //if (!councils.Any()) return HandlerFail("No available councils");
            var councils = await _userRepository.GetThreeCouncilsForIdeaRequest(command.IdeaVersionId.Value);
            if (!councils.Any()) return HandlerFail("No available councils");

            var newIdeaRequests = new List<IdeaVersionRequest>();

            foreach (var council in councils)
            {
                var ideaRequest = new IdeaVersionRequest
                {
                    //IdeaId = command.IdeaId,
                    //ReviewerId = council.Id,
                    //Status = IdeaRequestStatus.Pending,
                    //Role = "Council",
                };
                await SetBaseEntityForCreation(ideaRequest);
                newIdeaRequests.Add(ideaRequest);
            }

            if (!newIdeaRequests.Any()) return HandlerNotFound("No available councils");

            _ideaVersionRequestRepository.AddRange(newIdeaRequests);
            var check = await _unitOfWork.SaveChanges();
            if (check)
            {
                //sua db
                //var idea = await _ideaRepository.GetById((Guid)command.IdeaId);
                var idea = await _ideaRepository.GetById((Guid)command.IdeaVersionId);
                //send noti cho 3 nguoi council
                var request = new NotificationCreateForGroupUser
                {
                    Description = "Đề tài " + idea.Abbreviations + " đang chờ bạn duyệt với vai trò Council",
                };
                await _notificationService.CreateForGroupUsers(request, councils.Select(e => e.Id).ToList());
                //
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                   .WithMessage("Created council requests successfully.");
            }
            return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Failed to create council requests.");
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
            var ideaRequest = await _ideaVersionRequestRepository.GetById(command.Id);
            if (ideaRequest == null) return HandlerFail("Not found ideaRequest");

            ideaRequest.Status = command.Status;
            //sua db
            //ideaRequest.Content = command.Content;
            ideaRequest.ProcessDate = DateTime.UtcNow;
            _ideaVersionRequestRepository.Update(ideaRequest);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            // Nếu Mentor từ chối -> Idea bị từ chối ngay lập tức
            if (command.Status == IdeaVersionRequestStatus.Rejected && ideaRequest.Role == "Mentor")
            {
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                {
                    Id = ideaRequest.IdeaVersionId.Value,
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

    //public async Task<BusinessResult> CouncilResponse(IdeaRequestLecturerOrCouncilResponseCommand command)
    //{
    //    try
    //    {
    //        var user = await GetUserAsync();
    //        var ideaRequest = _mapper.Map<IdeaRequest>(command);
    //        var ideaRequestOld = await _ideaVersionRequestRepository.GetById(ideaRequest.Id);
    //        if (ideaRequestOld != null)
    //        {
    //            ideaRequestOld.Status = ideaRequest.Status;
    //            ideaRequestOld.Content = ideaRequest.Content;
    //            ideaRequestOld.ProcessDate = DateTime.UtcNow;
    //            ideaRequestOld.ReviewerId = user.Id;
    //            await SetBaseEntityForUpdate(ideaRequestOld);
    //            _ideaVersionRequestRepository.Update(ideaRequestOld);

    //            var idea = await _ideaRepository.GetById(ideaRequestOld.IdeaId.Value);


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
    //        var errorMessage = $"An error {typeof(IdeaRequestResult).Name}: {ex.Message}";
    //        return new ResponseBuilder()
    //            .WithStatus(Const.FAIL_CODE)
    //            .WithMessage(errorMessage);
    //    }
    //}

    public async Task<BusinessResult> LecturerResponse(IdeaVersionRequestLecturerOrCouncilResponseCommand command)
    {
        try
        {
            var ideaRequest = _mapper.Map<IdeaVersionRequest>(command);
            var ideaRequestOld = await _ideaVersionRequestRepository.GetById(ideaRequest.Id);
            if (ideaRequestOld != null)
            {
                ideaRequestOld.Status = ideaRequest.Status;
                //ideaRequestOld.Content = ideaRequest.Content;
                ideaRequestOld.ProcessDate = DateTime.UtcNow;
                await SetBaseEntityForUpdate(ideaRequestOld);
                _ideaVersionRequestRepository.Update(ideaRequestOld);

                //sua db
                //var idea = await _ideaRepository.GetById(ideaRequestOld.IdeaId.Value);
                var idea = await _ideaRepository.GetById(ideaRequestOld.IdeaVersionId.Value);
                if (idea == null) return HandlerFail("Idea not found");

                await SetBaseEntityForUpdate(idea);
                _ideaRepository.Update(idea);

                bool saveChange = await _unitOfWork.SaveChanges();
                if (saveChange)
                {
                    if (idea.MentorId == null)
                    {
                        return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("De tai khong co mentor id");
                    }
                    var mentor = await _userRepository.GetById((Guid)idea.MentorId);
                    if (mentor == null)
                    {
                        return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("De tai khong co mentor");
                    }
                    //noti cho owner
                    var noti = new NotificationCreateForIndividual
                    {
                        UserId = idea.OwnerId,
                        Description = "Đề tài " + idea.Abbreviations + " đã được " + mentor.Code + "(Mentor) duyệt. Hãy kiểm tra kết quả!",
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

            return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Not found idea");
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