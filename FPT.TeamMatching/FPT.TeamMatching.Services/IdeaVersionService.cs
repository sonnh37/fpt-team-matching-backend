using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Services
{
    public class IdeaVersionService : BaseService<IdeaVersion>, IIdeaVersionService
    {
        private readonly IIdeaRepository _ideaRepository;
        private readonly IIdeaVersionRepository _ideaVersionRepository;
        private readonly IIdeaVersionRequestRepository _ideaVersionRequestRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStageIdeaRepositoty _stageIdeaRepositoty;
        private readonly INotificationService _notificationService;

        public IdeaVersionService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
        {
            _ideaRepository = unitOfWork.IdeaRepository;
            _ideaVersionRepository = unitOfWork.IdeaVersionRepository;
            _ideaVersionRequestRepository = unitOfWork.IdeaVersionRequestRepository;
            _semesterRepository = unitOfWork.SemesterRepository;
            _userRepository = unitOfWork.UserRepository;
            _stageIdeaRepositoty = unitOfWork.StageIdeaRepository;
            _notificationService = notificationService;

        }

        public async Task<BusinessResult> ResubmitByStudentOrMentor(IdeaVersionResubmitByStudentOrMentor request)
        {
            try
            {
                //get user
                var user = await GetUserAsync();

                //lay ra stageIdea hien tai
                var stageIdea = await _stageIdeaRepositoty.GetCurrentStageIdea();
                if (stageIdea == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Không có đợt duyệt ứng với ngày hiện tại");
                }

                //ki hien tai
                var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
                if (semester == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Không có kì ứng với đợt duyệt hiện tại");
                }
                
                if (request.IdeaId == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhập idea id");
                }
                var idea = await _ideaRepository.GetById((Guid)request.IdeaId);
                if (idea == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
                }
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
                var saveChange = await _unitOfWork.SaveChanges();
                if (!saveChange)
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
                    saveChange = await _unitOfWork.SaveChanges();
                    if (!saveChange)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }
                
               

                //tao IdeaVersionRequest
                //neu la student -> tao cho mentor
                if (user.UserXRoles.Where(e => e.Role.RoleName == "Student").Any())
                {
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

                    //gửi noti cho mentor
                    var command = new NotificationCreateForIndividual
                    {
                        UserId = idea.MentorId,
                        Description = "Đề tài " + ideaVersion.Abbreviations + "(resubmit) đang chờ bạn duyệt với vai trò Mentor",
                    };
                    await _notificationService.CreateForUser(command);
                }
                else if (user.UserXRoles.Where(e => e.Role.RoleName == "Mentor").Any())
                {
                    //tao IdeaVersionRequest cho mentor (approved)
                    var ideaVersionRequestForMentor = new IdeaVersionRequest
                    {
                        IdeaVersionId = ideaVersion.Id,
                        ReviewerId = idea.MentorId,
                        CriteriaFormId = semester.CriteriaFormId,
                        Status = IdeaVersionRequestStatus.Approved,
                        Role = "Mentor",
                    };
                    await SetBaseEntityForCreation(ideaVersionRequestForMentor);
                    _ideaVersionRequestRepository.Add(ideaVersionRequestForMentor);

                    saveChange = await _unitOfWork.SaveChanges();
                    if (!saveChange)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }
                
                idea.Status = IdeaStatus.Pending;
                _ideaRepository.Update(idea);
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
                var errorMessage = $"An error occurred in {typeof(IdeaVersionResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
