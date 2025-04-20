using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersionRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Services
{
    public class TopicVersionRequestService : BaseService<TopicVersionRequest>, ITopicVersionRequestService
    {
        private readonly ITopicVersionRepository _topicVersionRepository;
        private readonly ITopicVersionRequestRepository _topicVersionRequestRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;

        public TopicVersionRequestService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
        {
            _topicVersionRepository = unitOfWork.TopicVersionRepository;
            _topicVersionRequestRepository = unitOfWork.TopicVersionRequestRepository;
            _projectRepository = unitOfWork.ProjectRepository;
            _notificationService = notificationService;
        }

        public async Task<BusinessResult> RespondByLecturerOrManager(RespondByMentorOrManager request)
        {
            try
            {
                var topicVersionRequest = await _topicVersionRequestRepository.GetById((Guid)request.Id);
                if (topicVersionRequest == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);
                }
                topicVersionRequest.Status = request.Status;
                topicVersionRequest.Feedback = request.Feedback;

                //neu la manager thi get user hien tai lam reviewerId
                if (topicVersionRequest.Role == "Manager")
                {
                    var user = await GetUserAsync();
                    topicVersionRequest.ReviewerId = user.Id;
                }
                await SetBaseEntityForUpdate(topicVersionRequest);
                _topicVersionRequestRepository.Update(topicVersionRequest);

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }

                var topicVersion = await _topicVersionRepository.GetById((Guid)topicVersionRequest.TopicVersionId);

                //neu la mentor -> noti -> neu mentor approve tạo tiếp TopicVersionRequest cho Manager
                if (topicVersionRequest.Role == "Mentor")
                {
                    //get project id
                    var project = await _projectRepository.GetProjectByTopicId((Guid)topicVersion.TopicId);
                    if (project == null)
                    {
                        return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Khong tim thay project");
                    }
                    //noti duyệt sửa đề tài -> team
                    var noti = new NotificationCreateForTeam
                    {
                        ProjectId = project.Id,
                        Description = "Yêu cầu chỉnh sửa đề tài sau review " + topicVersion.ReviewStage +
                                        " của nhóm bạn đã được Mentor duyệt. Hãy kiểm tra kết quả!",
                    };
                    await _notificationService.CreateForTeam(noti);

                    //neu mentor approve tạo tiếp TopicVersionRequest cho Manager
                    if (topicVersionRequest.Status == Domain.Enums.TopicVersionRequestStatus.Approved)
                    {
                        var requestForManager = new TopicVersionRequest
                        {
                            TopicVersionId = topicVersion.Id,
                            Status = Domain.Enums.TopicVersionRequestStatus.Pending,
                            Role = "Manager",
                        };
                        await SetBaseEntityForCreation(requestForManager);
                        _topicVersionRequestRepository.Add(requestForManager);

                        isSuccess = await _unitOfWork.SaveChanges();
                        if (!isSuccess)
                        {
                            return new ResponseBuilder()
                                .WithStatus(Const.FAIL_CODE)
                                .WithMessage(Const.FAIL_SAVE_MSG);
                        }
                    }

                }

                //neu la manager -> update TopicVersion va noti
                if (topicVersionRequest.Role == "Manager")
                {
                    if (topicVersionRequest.Status == Domain.Enums.TopicVersionRequestStatus.Approved)
                    {
                        topicVersion.Status = Domain.Enums.TopicVersionStatus.Approved;
                        await SetBaseEntityForUpdate(topicVersion);
                        _topicVersionRepository.Update(topicVersion);

                        isSuccess = await _unitOfWork.SaveChanges();
                        if (!isSuccess)
                        {
                            return new ResponseBuilder()
                                .WithStatus(Const.FAIL_CODE)
                                .WithMessage(Const.FAIL_SAVE_MSG);
                        }
                    }

                    else if (topicVersionRequest.Status == Domain.Enums.TopicVersionRequestStatus.Rejected)
                    {
                        topicVersion.Status = Domain.Enums.TopicVersionStatus.Rejected;
                        await SetBaseEntityForUpdate(topicVersion);
                        _topicVersionRepository.Update(topicVersion);

                        isSuccess = await _unitOfWork.SaveChanges();
                        if (!isSuccess)
                        {
                            return new ResponseBuilder()
                                .WithStatus(Const.FAIL_CODE)
                                .WithMessage(Const.FAIL_SAVE_MSG);
                        }
                    }

                    //get project id
                    var project = await _projectRepository.GetProjectByTopicId((Guid)topicVersion.TopicId);
                    //noti duyệt sửa đề tài -> team
                    var noti = new NotificationCreateForTeam
                    {
                        ProjectId = project.Id,
                        Description = "Yêu cầu chỉnh sửa đề tài sau review " + topicVersion.ReviewStage +
                                        " của nhóm bạn đã được duyệt xong. Hãy kiểm tra kết quả!",
                    };
                    await _notificationService.CreateForTeam(noti);
                    //
                    return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);

            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(TopicVersionRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
