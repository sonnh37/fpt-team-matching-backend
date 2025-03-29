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

namespace FPT.TeamMatching.Services
{
    public class MentorIdeaRequestService : BaseService<MentorIdeaRequest>, IMentorIdeaRequestService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IIdeaRepository _ideaRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IMentorIdeaRequestRepository _mentorIdeaRequestRepository;

        public MentorIdeaRequestService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _projectRepository = _unitOfWork.ProjectRepository;
            _ideaRepository = _unitOfWork.IdeaRepository;
            _teamMemberRepository = _unitOfWork.TeamMemberRepository;
            _mentorIdeaRequestRepository = _unitOfWork.MentorIdeaRequestRepository;
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
                    .WithMessage("Khong tim thay idea");
                }
                //check project exist
                var project = await _projectRepository.GetById(request.ProjectId);
                if (project == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage("Khong tim thay project");
                }
                //check team co toi thieu 4ng
                var tm = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                if (tm != null && tm.Count < 4)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhóm phải có tối thiểu 4 người");
                }
                var entity = new MentorIdeaRequest
                {
                    ProjectId = request.ProjectId,
                    IdeaId = request.IdeaId,
                    Status = Domain.Enums.MentorIdeaRequestStatus.Pending
                };
                _mentorIdeaRequestRepository.Add(entity);
                bool isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
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

        public async Task<BusinessResult> MentorResponse(MentorIdeaRequestUpdateCommand request)
        {
            try
            {
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

                //nếu reject -> update 1 reject
                if (request.Status == MentorIdeaRequestStatus.Rejected)
                {
                    mentorIdeaRequest.Status = MentorIdeaRequestStatus.Rejected;
                    _mentorIdeaRequestRepository.Update(mentorIdeaRequest);
                    await _unitOfWork.SaveChanges();

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
                        item.Status = (item.Id == request.Id) ? MentorIdeaRequestStatus.Approved : MentorIdeaRequestStatus.Rejected;
                        await SetBaseEntityForUpdate(item);
                    }
                    _mentorIdeaRequestRepository.UpdateRange(mentorIdeaRequests);
                    await _unitOfWork.SaveChanges();
                    //idea: cap nhat isExistedTeam
                    idea.IsExistedTeam = true;
                    _ideaRepository.Update(idea);
                    await _unitOfWork.SaveChanges();
                    //gan ideaId vao project
                    project.IdeaId = idea.Id;
                    _projectRepository.Update(project);
                    await _unitOfWork.SaveChanges();
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
