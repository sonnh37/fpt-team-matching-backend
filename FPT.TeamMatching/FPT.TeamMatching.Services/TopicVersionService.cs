using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersions;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersionRequests;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Services
{
    public class TopicVersionService : BaseService<TopicVersion>, ITopicVersionService
    {
        private readonly ITopicVersionRepository _topicVersionRepository;
        private readonly IIdeaRepository _ideaRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ITopicVersionRequestRepository _topicVersionRequestRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly INotificationService _notificationService;
        public TopicVersionService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
        {
            _topicVersionRepository = unitOfWork.TopicVersionRepository;
            _topicVersionRequestRepository = unitOfWork.TopicVersionRequestRepository;
            _ideaRepository = unitOfWork.IdeaRepository;
            _topicRepository = unitOfWork.TopicRepository;
            _reviewRepository = unitOfWork.ReviewRepository;
            _notificationService = notificationService;
        }

        

        public async Task<BusinessResult> GetAllTopicVersionByIdeaId(Guid ideaId)
        {
            try
            {
                var result = await _topicVersionRepository.GetAllByIdeaId(ideaId);
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithData(result)
                    .WithMessage(Const.SUCCESS_READ_MSG); 
            }
            catch (Exception e)
            {
                var errorMessage = $"An error occurred in {typeof(TopicVersionResult).Name}: {e.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> UpdateByStudent(UpdateTopicByStudentCommand request)
        {
            try
            {
                if (request.TopicId == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhập đề tài");
                }
                var topic = await _topicRepository.GetByTopicId(request.TopicId.Value);
                if (topic == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Đề tài không tồn tại");
                }

                var currentReview = await _reviewRepository.GetReviewByProjectIdAndNumber(topic.Project.Id, request.ReviewStage);
                if (currentReview == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage($"Không tìm thấy review của giai đoạn {request.ReviewStage}");
                }
                //Kiểm tra ngày nộp có trước review stage tiếp theo hay không
                var nextReview = await _reviewRepository.GetReviewByProjectIdAndNumber(topic.Project.Id, request.ReviewStage + 1);
                if (nextReview == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Đề tài không có review tiếp theo");
                }

                if (nextReview.ReviewDate != null && nextReview.ReviewDate.Value.LocalDateTime >= DateTime.UtcNow.ToLocalTime())
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Đề tài đã qua review tiếp theo không thể cập nhât");
                }

                if (DateTime.UtcNow.ToLocalTime() < currentReview.ReviewDate.Value.LocalDateTime)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Chưa tới thời gian để yêu cầu chỉnh sửa");
                    
                }
                //tao TopicVersion
                var topicVersion = new TopicVersion
                {
                    Id = Guid.NewGuid(),
                    TopicId = topic.Id,
                    Status = Domain.Enums.TopicVersionStatus.Pending,
                    FileUpdate = request.FileUpdate,
                    ReviewStage = request.ReviewStage,
                    Note = request.Note,
                };
                await SetBaseEntityForCreation(topicVersion);
                _topicVersionRepository.Add(topicVersion);

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }
                //tao TopicVersionRequest
                var topicVersionRequest = new TopicVersionRequest
                {
                    TopicVersionId = topicVersion.Id,
                    ReviewerId = topic.IdeaVersion.Idea.MentorId,
                    Status = TopicVersionRequestStatus.Pending,
                    Role = "Mentor"
                };
                await SetBaseEntityForCreation(topicVersionRequest);
                _topicVersionRequestRepository.Add(topicVersionRequest);

                isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }

                //noti chỉnh sửa đề tài cho mentor
                var noti = new NotificationCreateForIndividual
                {
                    UserId = topic.IdeaVersion.Idea.MentorId,
                    Description = "Đề tài " + topic.IdeaVersion.Abbreviations + " gửi yêu cầu chỉnh sửa sau review " + topicVersion.ReviewStage,
                };
                await _notificationService.CreateForUser(noti);
                //
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(TopicVersionResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
