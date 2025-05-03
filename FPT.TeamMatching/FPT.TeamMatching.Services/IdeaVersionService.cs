using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersions;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services
{
    public class IdeaVersionService : BaseService<IdeaVersion>, IIdeaVersionService
    {
        private readonly IIdeaRepository _ideaRepository;
        private readonly IIdeaVersionRepository _ideaVersionRepository;
        private readonly IIdeaVersionRequestRepository _ideaVersionRequestRepository;
        private readonly IIdeaVersionRequestService _ideaVersionRequestService;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStageIdeaRepositoty _stageIdeaRepositoty;
        private readonly INotificationService _notificationService;

        public IdeaVersionService(IMapper mapper,
            IUnitOfWork unitOfWork,
            IIdeaVersionRequestService ideaVersionRequestService,
            INotificationService notificationService) :
            base(mapper, unitOfWork)
        {
            _ideaRepository = unitOfWork.IdeaRepository;
            _ideaVersionRepository = unitOfWork.IdeaVersionRepository;
            _ideaVersionRequestRepository = unitOfWork.IdeaVersionRequestRepository;
            _semesterRepository = unitOfWork.SemesterRepository;
            _userRepository = unitOfWork.UserRepository;
            _stageIdeaRepositoty = unitOfWork.StageIdeaRepository;
            _notificationService = notificationService;
            _ideaVersionRequestService = ideaVersionRequestService;
        }

        public async Task<BusinessResult> ResubmitByStudentOrMentor(IdeaVersionResubmitByStudentOrMentor request)
        {
            try
            {
                var semesterUpComing = await _semesterRepository.GetUpComingSemester();
                var student = await GetUserByRole("Student", semesterUpComing?.Id);

                var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
                if (stageIdea == null) return HandlerFail("Không có đợt duyệt ứng với ngày hiện tại");

                var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
                if (semester == null) return HandlerFail("Không có kì ứng với đợt duyệt hiện tại");

                if (request.IdeaId == null) return HandlerFail("Nhap IdeaId");

                var idea = await _ideaRepository.GetById((Guid)request.IdeaId);
                if (idea == null) return HandlerNotFound();

                // Check có ideaVersion cũ có topic ko
                var ideaVersionsOfIdea = await _ideaVersionRepository.GetIdeaVersionsByIdeaId(idea.Id);
                var ideaVersionListId = ideaVersionsOfIdea.Select(m => m.Id).ToList().ConvertAll<Guid?>(x => x);
                var existingTopics = await _unitOfWork.TopicRepository.GetTopicByIdeaVersionId(ideaVersionListId);
                //tao IdeaVersion
                var ideaVersion = _mapper.Map<IdeaVersion>(request);
                ideaVersion.Id = Guid.NewGuid();
                ideaVersion.Version = ideaVersionsOfIdea.Count + 1;
                ideaVersion.StageIdeaId = stageIdea.Id;
                await SetBaseEntityForCreation(ideaVersion);
                _ideaVersionRepository.Add(ideaVersion);
                if (!await _unitOfWork.SaveChanges())
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }

                if (existingTopics.Count > 0)
                {
                    var topic = existingTopics[0];
                    topic.IdeaVersionId = ideaVersion.Id;
                    _unitOfWork.TopicRepository.Update(topic);

                    if (!await _unitOfWork.SaveChanges())
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }


                //tao IdeaVersionRequest
                await _ideaVersionRequestService.CreateVersionRequests(idea, ideaVersion.Id,
                    semester.CriteriaFormId.Value);
                if (!await _unitOfWork.SaveChanges())
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }

                if (student != null)
                {
                    var command = new NotificationCreateForIndividual
                    {
                        UserId = idea.MentorId,
                        Description = "Đề tài " + ideaVersion.Abbreviations +
                                      "(resubmit) đang chờ bạn duyệt với vai trò Mentor",
                    };
                    await _notificationService.CreateForUser(command);
                }

                idea.Status = IdeaStatus.Pending;
                _ideaRepository.Update(idea);
                if (!await _unitOfWork.SaveChanges())
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
                var errorMessage = $"An error occurred in {typeof(IdeaVersionResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}