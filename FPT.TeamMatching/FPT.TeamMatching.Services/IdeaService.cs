using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FPT.TeamMatching.Services;

public class IdeaService : BaseService<Idea>, IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IIdeaRequestRepository _ideaRequestRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStageIdeaRepositoty _stageIdeaRepositoty;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public IdeaService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _ideaRepository = unitOfWork.IdeaRepository;
        _ideaRequestRepository = unitOfWork.IdeaRequestRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _userRepository = unitOfWork.UserRepository;
        _stageIdeaRepositoty = unitOfWork.StageIdeaRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
    }


    public async Task<BusinessResult> GetIdeasByUserId()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var ideas = await _ideaRepository.GetIdeasByUserId(userId.Value);
                var result = _mapper.Map<IList<IdeaResult>>(ideas);
                if (result == null)
                    return new ResponseBuilder()
                        .WithData(result)
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);

                return new ResponseBuilder()
                    .WithData(result)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> StudentCreatePending(IdeaStudentCreatePendingCommand idea)
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

            //ki hien tai
            var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có kì ứng với đợt duyệt hiện tại");
            }

            //check student co idea approve trong ki nay k
            var userId = GetUserIdFromClaims();
            var ideaApprovedInSemester =
                await _ideaRepository.GetIdeaApproveInSemesterOfUser((Guid)userId, semester.Id);
            if (ideaApprovedInSemester != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Sinh viên đã có đề tài được duyệt trong kì này");
            }

            //check student co idea dang pending trong dot duyet nay k
            var ideaPendingInStageIdea =
                await _ideaRepository.GetIdeaPendingInStageIdeaOfUser((Guid)userId, (Guid)stageIdea.Id);
            if (ideaPendingInStageIdea != null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Sinh viên có đề tài đang trong quá trình duyệt ở kì này");
            }

            var ideaEntity = _mapper.Map<Idea>(idea);
            if (ideaEntity == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            ideaEntity.StageIdeaId = stageIdea.Id;
            ideaEntity.Status = IdeaStatus.Pending;
            ideaEntity.OwnerId = userId;
            ideaEntity.Type = IdeaType.Student;
            ideaEntity.IsExistedTeam = true;
            ideaEntity.IsEnterpriseTopic = false;
            await SetBaseEntityForCreation(ideaEntity);
            _ideaRepository.Add(ideaEntity);

            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            var ideaRequest = new IdeaRequest
            {
                IdeaId = ideaEntity.Id,
                ReviewerId = ideaEntity.MentorId,
                ProcessDate = DateTime.UtcNow,
                Status = IdeaRequestStatus.Pending,
                Role = "Mentor",
            };
            await SetBaseEntityForCreation(ideaRequest);
            _ideaRequestRepository.Add(ideaRequest);

            var _saveChange = await _unitOfWork.SaveChanges();
            if (!_saveChange)
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
            var errorMessage = $"An error occurred while updating : {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> LecturerCreatePending(IdeaLecturerCreatePendingCommand idea)
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

            //ki hien tai
            var semester = await _semesterRepository.GetSemesterByStageIdeaId(stageIdea.Id);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có kì ứng với đợt duyệt hiện tại");
            }

            //check đề tài đki thứ 5 phải có submentor
            var userId = GetUserIdFromClaims();
            var numberOfIdeaMentorOrOwner = await _ideaRepository.NumberOfIdeaMentorOrOwner((Guid)userId);
            if (numberOfIdeaMentorOrOwner > 4)
            {
                //k co submentor
                if (idea.SubMentorId == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Lecturer is mentor or owner in 4 ideas, the 5th idea needs submentor");
                }

                //k tim thay submentor
                var submentor = await _userRepository.GetById((Guid)idea.SubMentorId);
                if (submentor == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Don't exist submentor with given id");
                }
            }

            //check đề tài doanh nghiệp thì phải nhập tên doanh nghiệp
            if (idea.IsEnterpriseTopic)
            {
                if (idea.EnterpriseName == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Enterprise idea need enterprise name");
                }
            }

            var ideaEntity = _mapper.Map<Idea>(idea);
            if (ideaEntity == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            ideaEntity.Status = IdeaStatus.Pending;
            ideaEntity.StageIdeaId = stageIdea.Id;
            ideaEntity.OwnerId = userId;
            ideaEntity.MentorId = userId;
            ideaEntity.IsExistedTeam = false;
            if (idea.IsEnterpriseTopic)
            {
                ideaEntity.Type = IdeaType.Enterprise;
            }
            else
            {
                ideaEntity.Type = IdeaType.Lecturer;
                ideaEntity.EnterpriseName = null;
            }

            await SetBaseEntityForCreation(ideaEntity);
            _ideaRepository.Add(ideaEntity);

            var saveChange_ = await _unitOfWork.SaveChanges();
            if (!saveChange_)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            var ideaRequest = new IdeaRequest
            {
                IdeaId = ideaEntity.Id,
                ReviewerId = ideaEntity.MentorId,
                Status = IdeaRequestStatus.Approved,
                ProcessDate = DateTime.UtcNow,
                Role = "Mentor",
            };
            await SetBaseEntityForCreation(ideaRequest);
            _ideaRequestRepository.Add(ideaRequest);

            var saveChange = await _unitOfWork.SaveChanges();
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
            var errorMessage = $"An error occurred while updating : {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetCurrentIdeaByStatus(IdeaGetCurrentByStatus query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerError("User does not exist");

            if (query.Status == null) return HandlerFail("Status cannot be null");

            var ideas = await _ideaRepository.GetCurrentIdeaByUserIdAndStatus(userId.Value, query.Status.Value);
            var result = _mapper.Map<List<IdeaResult>>(ideas);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> UpdateIdea(IdeaUpdateCommand ideaUpdateCommand)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var project = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
            if (project == null || project.Idea == null) return HandlerFail("Not found project");

            ideaUpdateCommand.OwnerId = project.Idea.OwnerId;
            ideaUpdateCommand.MentorId = project.Idea.MentorId;
            ideaUpdateCommand.SubMentorId = project.Idea.SubMentorId;
            ideaUpdateCommand.IdeaCode = project.Idea.IdeaCode;
            ideaUpdateCommand.SpecialtyId = project.Idea.SpecialtyId;
            ideaUpdateCommand.IsExistedTeam = project.Idea.IsExistedTeam;
            ideaUpdateCommand.IsEnterpriseTopic = project.Idea.IsEnterpriseTopic;
            ideaUpdateCommand.EnterpriseName = project.Idea.EnterpriseName;
            ideaUpdateCommand.MaxTeamSize = project.Idea.MaxTeamSize;

            var command = _mapper.Map<Idea>(ideaUpdateCommand);
            command.Id = project.Idea.Id;
            _ideaRepository.Update(command);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> UpdateStatusIdea(IdeaUpdateStatusCommand command)
    {
        try
        {
            var idea = await _ideaRepository.GetById(command.Id);
            if (idea == null) return HandlerFail("Not found idea");
            idea.Status = command.Status;
            _ideaRepository.Update(idea);
            var check = await _unitOfWork.SaveChanges();

            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithData(idea)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task AutoUpdateIdeaStatus()
    {
        try
        {
            var ideas = await _ideaRepository.GetIdeaWithResultDateIsToday();
            if (ideas.Count != 0)
            {
                foreach (var idea in ideas)
                {
                    var totalCouncils = await _ideaRequestRepository.CountCouncilsForIdea(idea.Id);
                    var totalApproved = await _ideaRequestRepository.CountApprovedCouncilsForIdea(idea.Id);
                    var totalRejected = await _ideaRequestRepository.CountRejectedCouncilsForIdea(idea.Id);

                    if (totalCouncils == 3)
                    {
                        if (totalApproved > totalRejected)
                            await UpdateIdea(idea, IdeaStatus.Approved);
                        if (totalRejected > totalApproved)
                            await UpdateIdea(idea, IdeaStatus.Rejected);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }

    private async Task UpdateIdea(Idea idea, IdeaStatus status)
    {
        try
        {
            if (idea.StageIdeaId != null)
            {
                if (status == IdeaStatus.Approved)
                {
                    //Gen idea code 
                    var stageIdea = await _stageIdeaRepositoty.GetById((Guid)idea.StageIdeaId);
                    var semester = await _semesterRepository.GetById((Guid)stageIdea.SemesterId);
                    if (semester == null) return;
                    var semesterCode = semester.SemesterCode;
                    var semesterPrefix = semester.SemesterPrefixName;
                    //get so luong idea dc duyet approve cua ki
                    var numberOfIdeas = await _ideaRepository.NumberApprovedIdeasOfSemester(semester.Id);

                    // Tạo số thứ tự tiếp theo
                    int nextNumberIdea = numberOfIdeas + 1;

                    // Tạo mã Idea mới theo định dạng: semesterPrefix + semesterCode + "SE" + số thứ tự (2 chữ số)
                    string newIdeaCode = $"{semesterPrefix}{semesterCode}SE{nextNumberIdea:D2}";

                    idea.IdeaCode = newIdeaCode;
                    //Check neu owner la student thi tao project
                    var isStudent = idea.Owner.UserXRoles.Any(e => e.Role.RoleName == "Student");
                    if (isStudent)
                    {
                        //check xem co team chua - co project chua
                        var existedProject = await _projectRepository.GetProjectByLeaderId((Guid)idea.OwnerId);
                        if (existedProject == null)
                        {
                            //Tao project
                            var project = new Project
                            {
                                LeaderId = isStudent ? idea.OwnerId : null,
                                IdeaId = idea.Id,
                                //TeamCode = newTeamCode,
                                Status = ProjectStatus.Pending,
                                TeamSize = idea.MaxTeamSize
                            };
                            await SetBaseEntityForCreation(project);
                            _projectRepository.Add(project);
                            var isSuccess = await _unitOfWork.SaveChanges();
                            if (!isSuccess)
                            {
                                return;
                            }
                            //Tao teamMember
                            var teamMember = new TeamMember
                            {
                                UserId = idea.OwnerId,
                                ProjectId = project.Id,
                                Role = TeamMemberRole.Leader,
                                JoinDate = DateTime.UtcNow,
                                Status = TeamMemberStatus.Pending,
                            };
                            await SetBaseEntityForCreation(teamMember);
                            _teamMemberRepository.Add(teamMember);
                            isSuccess = await _unitOfWork.SaveChanges();
                            if (!isSuccess)
                            {
                                return;
                            }
                        }
                        else
                        {
                            existedProject.IdeaId = idea.Id;
                            await SetBaseEntityForUpdate(existedProject);
                            _projectRepository.Update(existedProject);
                            var isSuccess = await _unitOfWork.SaveChanges();
                            if (!isSuccess)
                            {
                                return;
                            }
                        }

                    }
                }
                //update idea
                idea.Owner = null;
                idea.StageIdea = null;
                idea.Status = status;
                await SetBaseEntityForUpdate(idea);
                _ideaRepository.Update(idea);
                var isSucess = await _unitOfWork.SaveChanges();
                if (!isSucess)
                {
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("error update idea" + ex);
        }
    }

    public async Task AutoUpdateProjectInProgress()
    {
        try
        {
            //get projects cua ki den ngay bat dau inprogress - ngay khoa
            var projects = await _projectRepository.GetProjectsInFourthWeekByToday();
            //update project
            foreach (var project in projects)
            {
                var semester = project.Idea.StageIdea.Semester;
                //update status
                project.Status = ProjectStatus.InProgress;
                //update teamCode
                    //get so luong project InProgress cua ki
                    var numberOfProjects = await _projectRepository.NumberOfInProgressProjectInSemester(semester.Id);
                    // Tạo số thứ tự tiếp theo
                    int nextNumber = numberOfProjects + 1;
                    string semesterCode = semester.SemesterCode;
                    //Tạo mã nhóm
                    string newTeamCode = $"{semesterCode}SE{nextNumber:D3}";
                    project.Idea = null;
                await SetBaseEntityForUpdate(project);
                _projectRepository.Update(project);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return;
                }
                //update status cua teammember
                var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId(project.Id);
                foreach (var teamMember in teamMembers)
                {
                    await SetBaseEntityForUpdate(teamMember);
                    teamMember.Status = TeamMemberStatus.InProgress;
                }
                _teamMemberRepository.UpdateRange(teamMembers);
                isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
}