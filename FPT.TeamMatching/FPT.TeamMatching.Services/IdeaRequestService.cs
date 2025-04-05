using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using NetTopologySuite.Triangulate.Tri;

namespace FPT.TeamMatching.Services;

public class IdeaRequestService : BaseService<IdeaRequest>, IIdeaRequestService
{
    private readonly IIdeaRequestRepository _ideaRequestRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIdeaService _ideaService;
    private readonly INotificationService _notificationService;

    public IdeaRequestService(IMapper mapper, IUnitOfWork unitOfWork, IIdeaService ideaService, INotificationService notificationService) : base(mapper,
        unitOfWork)
    {
        _ideaRequestRepository = unitOfWork.IdeaRequestRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _userRepository = unitOfWork.UserRepository;
        _ideaService = ideaService;
        _notificationService = notificationService;
    }

    public async Task<BusinessResult>
        GetAll<TResult>(IdeaRequestGetAllQuery query) where TResult : BaseResult
    {
        try
        {
            var (data, total) = await _ideaRequestRepository.GetData(query);

            var results = _mapper.Map<List<TResult>>(data);

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);

            // GetAll with pagination
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                .WithData(tableResponse)
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
        IdeaRequestGetListByStatusAndRoleQuery query) where TResult : BaseResult
    {
        try
        {
            var userIdClaims = GetUserIdFromClaims();
            var userId = userIdClaims.Value;
            var (data, total) = await _ideaRequestRepository.GetIdeaRequestsForCurrentReviewerByRolesAndStatus(query, userId);

            var results = _mapper.Map<List<TResult>>(data);

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);

            // GetAll with pagination
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                .WithData(tableResponse)
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
            var (data, total) = await _ideaRequestRepository.GetDataUnassignedReviewer(query);

            var results = _mapper.Map<List<TResult>>(data);

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);

            // GetAll with pagination
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                .WithData(tableResponse)
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

    public async Task<BusinessResult> CreateCouncilRequestsForIdea(IdeaRequestCreateForCouncilsCommand command)
    {
        try
        {
            if (command.IdeaId == Guid.Empty || command.IdeaId == null) return HandlerFail("No Idea Id provided");
            var councils = await _userRepository.GetThreeCouncilsForIdeaRequest(command.IdeaId.Value);
            if (!councils.Any()) return HandlerFail("No available councils");

            var newIdeaRequests = new List<IdeaRequest>();

            foreach (var council in councils)
            {
                var ideaRequest = new IdeaRequest
                {
                    IdeaId = command.IdeaId,
                    ReviewerId = council.Id,
                    Status = IdeaRequestStatus.Pending,
                    Role = "Council",
                };
                await SetBaseEntityForCreation(ideaRequest);
                newIdeaRequests.Add(ideaRequest);
            }

            if (!newIdeaRequests.Any()) return HandlerNotFound("No available councils");

            _ideaRequestRepository.AddRange(newIdeaRequests);
            var check = await _unitOfWork.SaveChanges();
            if (check)
            {
                var idea = await _ideaRepository.GetById((Guid)command.IdeaId);
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

    public async Task<BusinessResult> UpdateStatus(IdeaRequestUpdateStatusCommand command)
    {
        try
        {
            var ideaRequest = await _ideaRequestRepository.GetById(command.Id);
            if (ideaRequest == null) return HandlerFail("Not found ideaRequest");

            ideaRequest.Status = command.Status;
            ideaRequest.Content = command.Content;
            ideaRequest.ProcessDate = DateTime.UtcNow;
            _ideaRequestRepository.Update(ideaRequest);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            // Nếu Mentor từ chối -> Idea bị từ chối ngay lập tức
            if (command.Status == IdeaRequestStatus.Rejected && ideaRequest.Role == "Mentor")
            {
                return await _ideaService.UpdateStatusIdea(new IdeaUpdateStatusCommand
                {
                    Id = ideaRequest.IdeaId.Value,
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
        var totalCouncils = await _ideaRequestRepository.CountCouncilsForIdea(ideaId);
        var totalApproved = await _ideaRequestRepository.CountApprovedCouncilsForIdea(ideaId);
        var totalRejected = await _ideaRequestRepository.CountRejectedCouncilsForIdea(ideaId);

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
    //        var ideaRequestOld = await _ideaRequestRepository.GetById(ideaRequest.Id);
    //        if (ideaRequestOld != null)
    //        {
    //            ideaRequestOld.Status = ideaRequest.Status;
    //            ideaRequestOld.Content = ideaRequest.Content;
    //            ideaRequestOld.ProcessDate = DateTime.UtcNow;
    //            ideaRequestOld.ReviewerId = user.Id;
    //            await SetBaseEntityForUpdate(ideaRequestOld);
    //            _ideaRequestRepository.Update(ideaRequestOld);

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

    public async Task<BusinessResult> LecturerResponse(IdeaRequestLecturerOrCouncilResponseCommand command)
    {
        try
        {
            var ideaRequest = _mapper.Map<IdeaRequest>(command);
            var ideaRequestOld = await _ideaRequestRepository.GetById(ideaRequest.Id);
            if (ideaRequestOld != null)
            {
                ideaRequestOld.Status = ideaRequest.Status;
                ideaRequestOld.Content = ideaRequest.Content;
                ideaRequestOld.ProcessDate = DateTime.UtcNow;
                await SetBaseEntityForUpdate(ideaRequestOld);
                _ideaRequestRepository.Update(ideaRequestOld);

                var idea = await _ideaRepository.GetById(ideaRequestOld.IdeaId.Value, true);
                if (idea == null) return HandlerFail("Idea not found");
                // # Lỗi do thay đổi db
                // if (ideaRequestOld.Status == IdeaRequestStatus.MentorRejected)
                // {
                //     idea.Status = IdeaStatus.Rejected;
                // }

                await SetBaseEntityForUpdate(idea);
                _ideaRepository.Update(idea);

                bool saveChange = await _unitOfWork.SaveChanges();
                if (saveChange)
                {
                    //noti cho owner
                    var noti = new NotificationCreateForIndividual
                    {
                        UserId = idea.OwnerId,
                        Description = "Đề tài " + idea.Abbreviations + " đã được " + idea.Mentor.Code + "(Mentor) duyệt. Hãy kiểm tra kết quả!",
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
            var errorMessage = $"An error {typeof(IdeaRequestResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}