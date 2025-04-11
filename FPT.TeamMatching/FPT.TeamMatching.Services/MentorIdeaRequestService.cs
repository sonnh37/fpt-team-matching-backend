using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorIdeaRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorIdeaRequest;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using NetTopologySuite.Triangulate.Tri;

namespace FPT.TeamMatching.Services
{
    public class MentorIdeaRequestService : BaseService<MentorIdeaRequest>, IMentorIdeaRequestService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IIdeaRepository _ideaRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IMentorIdeaRequestRepository _mentorIdeaRequestRepository;
        private readonly INotificationService _notificationService;

        public MentorIdeaRequestService(IMapper mapper, IUnitOfWork unitOfWork,
            INotificationService notificationService) : base(mapper, unitOfWork)
        {
            _projectRepository = _unitOfWork.ProjectRepository;
            _ideaRepository = _unitOfWork.IdeaRepository;
            _teamMemberRepository = _unitOfWork.TeamMemberRepository;
            _mentorIdeaRequestRepository = _unitOfWork.MentorIdeaRequestRepository;
            _notificationService = notificationService;
        }

        public async Task<BusinessResult> StudentRequestIdea(StudentRequest request)
        {
            try
            {
                //check idea exist
                var idea = await _ideaRepository.GetById(request.IdeaId);
                if (idea == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy đề tài");
                }

                //check project exist
                var project = await _projectRepository.GetById(request.ProjectId);
                if (project == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy nhóm");
                }

                //check team co toi thieu 4ng
                var tm = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                if (tm != null && tm.Count < 4)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Đề tài " + idea.Abbreviations + " yêu cầu nhóm có " + idea.MaxTeamSize + " thành viên. Nhóm hiện tại chưa đáp ứng điều kiện này.");
                }

                var entity = new MentorIdeaRequest
                {
                    ProjectId = request.ProjectId,
                    IdeaId = request.IdeaId,
                    Status = Domain.Enums.MentorIdeaRequestStatus.Pending
                };
                await SetBaseEntityForUpdate(entity);
                _mentorIdeaRequestRepository.Add(entity);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
                    //noti cho mentor
                    var noti = new NotificationCreateForIndividual
                    {
                        UserId = idea.MentorId,
                        Description = "Nhóm " + project.TeamName + " đã gửi yêu cầu sử dụng đề tài đến bạn"
                    };
                    await _notificationService.CreateForUser(noti);

                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_DELETE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(MentorIdeaRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetUserMentorIdeaRequests(MentorIdeaRequestGetAllQuery query)
        {
            try
            {
                var userIdClaim = GetUserIdFromClaims();

                if (userIdClaim == null)
                    return HandlerFailAuth();

                var userId = userIdClaim.Value;
                var (data, total) = await _mentorIdeaRequestRepository.GetUserMentorIdeaRequests(query, userId);
                var results = _mapper.Map<List<MentorIdeaRequestResult>>(data);
                var response = new QueryResult(query, results, total);

                return new ResponseBuilder()
                    .WithData(response)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(MentorIdeaRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetMentorMentorIdeaRequests(MentorIdeaRequestGetAllQuery query)
        {
            try
            {
                var userIdClaim = GetUserIdFromClaims();
                if (userIdClaim == null)
                    return HandlerFailAuth();
                
                var userId = userIdClaim.Value;
                var (data, total) = await _mentorIdeaRequestRepository.GetMentorMentorIdeaRequests(query, userId);
                var results = _mapper.Map<List<MentorIdeaRequestResult>>(data);
                var response = new QueryResult(query, results, total);

                return new ResponseBuilder()
                    .WithData(response)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(MentorIdeaRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> UpdateMentorIdeaRequestStatus(MentorIdeaRequestUpdateCommand request)
        {
            try
            {
                var iSaveChanges = false;
                if (request.ProjectId == null || request.IdeaId == null || request.Status == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Nhập không đủ field");
                }

                var mentorIdeaRequest = await _mentorIdeaRequestRepository.GetById(request.Id);
                var project = await _projectRepository.GetById((Guid)request.ProjectId);
                var idea = await _ideaRepository.GetById((Guid)request.IdeaId);

                if (mentorIdeaRequest == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy request");
                }

                if (project == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy team");
                }

                if (idea == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy đề tài");
                }

                if (idea.MentorId == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("De tai khong co mentor id");
                }

                var mentor = await _unitOfWork.UserRepository.GetById(idea.MentorId.Value);

                if (mentor == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("De tai khong co mentor");
                }

                //nếu reject -> update 1 reject
                if (request.Status == MentorIdeaRequestStatus.Rejected)
                {
                    mentorIdeaRequest.Status = MentorIdeaRequestStatus.Rejected;
                    await SetBaseEntityForUpdate(mentorIdeaRequest);
                    _mentorIdeaRequestRepository.Update(mentorIdeaRequest);
                    iSaveChanges = await _unitOfWork.SaveChanges();
                    if (!iSaveChanges) return HandlerFail("Failed to save changes for approved request");

                    //noti cho nhom -> noti cho tung thanh vien
                    var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                    if (teamMembers == null)
                    {
                        return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Không tìm thấy thành viên của nhóm");
                    }
                    var noti = new NotificationCreateForIndividual
                    {
                        Description = "Mentor " + mentor.Code +
                                      "  đã duyệt yêu cầu sử dụng đề tài của nhóm bạn. Hãy kiểm tra!"
                    };
                    foreach (var member in teamMembers)
                    {
                        noti.UserId = member.Id;
                        await _notificationService.CreateForUser(noti);
                    }
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                //nếu apprrove -> update 1 approve, others reject 
                else if (request.Status == MentorIdeaRequestStatus.Approved)
                {
                    //lấy ra tất cả request cua idea do
                    var mentorIdeaRequests = await _mentorIdeaRequestRepository.GetByIdeaId((Guid)request.IdeaId);
                    //update
                    foreach (var item in mentorIdeaRequests)
                    {
                        item.Status = (item.Id == request.Id)
                            ? MentorIdeaRequestStatus.Approved
                            : MentorIdeaRequestStatus.Rejected;
                        await SetBaseEntityForUpdate(item);
                    }

                    _mentorIdeaRequestRepository.UpdateRange(mentorIdeaRequests);
                    iSaveChanges = await _unitOfWork.SaveChanges();
                    if (!iSaveChanges)
                        return HandlerFail("Failed to save changes for update range mentor idea request");

                    //idea: cap nhat isExistedTeam
                    idea.IsExistedTeam = true;
                    await SetBaseEntityForUpdate(idea);
                    _ideaRepository.Update(idea);
                    iSaveChanges = await _unitOfWork.SaveChanges();
                    if (!iSaveChanges) return HandlerFail("Failed to save changes for update idea");

                    //gan ideaId vao project
                    project.IdeaId = idea.Id;
                    await SetBaseEntityForUpdate(project);
                    _projectRepository.Update(project);
                    iSaveChanges = await _unitOfWork.SaveChanges();
                    if (!iSaveChanges) return HandlerFail("Failed to save changes for update project");

                    //noti cho nhom -> noti cho tung thanh vien
                    var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                    if (teamMembers == null)
                    {
                        return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Không tìm thấy thành viên của nhóm");
                    }
                    var noti = new NotificationCreateForIndividual
                    {
                        Description = "Mentor " + mentor.Code +
                                      "  đã duyệt yêu cầu sử dụng đề tài của nhóm bạn. Hãy kiểm tra!"
                    };
                    foreach (var member in teamMembers)
                    {
                        noti.UserId = member.Id;
                        await _notificationService.CreateForUser(noti);
                    }

                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(MentorIdeaRequestResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}