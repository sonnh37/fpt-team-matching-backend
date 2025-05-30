using AutoMapper;
using ClosedXML.Excel;
using CloudinaryDotNet.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System.Data;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

namespace FPT.TeamMatching.Services;

public class ProjectService : BaseService<Project>, IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly ITeamMemberService _serviceTeam;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ISemesterService _semesterService;
    private readonly IStageTopicRepository _stageTopicRepository;
    private readonly INotificationService _notificationService;
    private readonly ITopicRepository _topicRepository;

    public ProjectService(IMapper mapper, IUnitOfWork unitOfWork, ITeamMemberService teamMemberService, ISemesterService semesterService, INotificationService notificationService) : base(mapper,
        unitOfWork)
    {
        _projectRepository = unitOfWork.ProjectRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _serviceTeam = teamMemberService;
        _reviewRepository = unitOfWork.ReviewRepository;
        _semesterService = semesterService;
        _stageTopicRepository = unitOfWork.StageTopicRepository;
        _notificationService = notificationService;
        _topicRepository = _unitOfWork.TopicRepository;
    }

    public async Task<BusinessResult> GetProjectsForMentor(ProjectGetListForMentorQuery query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFailAuth();

            var (data, total) = await _projectRepository.GetProjectsForMentor(query, userId.Value);
            var results = _mapper.Map<List<ProjectResult>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> SearchProjects(ProjectSearchQuery query)
    {
        try
        {
            List<ProjectResult>? results;

            var wsSemester = await GetSemesterInCurrentWorkSpace();
            var (data, total) = await _projectRepository.SearchProjects(query, wsSemester.Id);

            results = _mapper.Map<List<ProjectResult>>(data);

            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetProjectByUserIdLogin()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFailAuth();

            var project = await _projectRepository.GetProjectByUserIdLoginFollowNewest(userId.Value);
            if (project == null) return HandlerFail("Người dùng không có project đang tồn tại");

            if (project.Status == ProjectStatus.Canceled)
                return HandlerFail("Người dùng không có project đang tồn tại trong kỳ này");

            var result = _mapper.Map<ProjectResult>(project);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> GetProjectInSemesterCurrentByUserIdLogin()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFailAuth();

            var semester = await GetSemesterInCurrentWorkSpace();

            var project = await _projectRepository.GetProjectInSemesterByUserIdLoginFollowNewest(userId.Value, semester?.Id);
            if (project == null) return HandlerFail("Người dùng không có nhóm đang tồn tại");

            if (project.Status == ProjectStatus.Canceled)
                return HandlerFail("Người dùng không có nhóm đang tồn tại trong này");

            var result = _mapper.Map<ProjectResult>(project);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> CreateProjectAndTeamMember(TeamCreateCommand command)
    {
        try
        {
            //lay ra stageTopic hien tai
            //var stageTopic = await _stageTopicRepository.GetCurrentStageTopic();
            //if (stageTopic == null) return HandlerFail("Không có đợt duyệt ứng với ngày hiện tại!");

            //ki cua stage idea
            //var semester = await _semesterRepository.GetSemesterByStageTopicId(stageTopic.Id);
            //if (semester == null) return HandlerFail("Không có kì ứng với đợt duyệt hiện tại!");

            var semesterCurrent = await GetSemesterInCurrentWorkSpace();
            if (semesterCurrent?.Status != SemesterStatus.Preparing)
                return HandlerFail("Hiện tại không được tạo nhóm");

            var newTeamCode = await _semesterService.GenerateNewTeamCode();

            //var codeExist = await _projectRepository.IsExistedTeamCode(newTeamCode);
            //if (codeExist) return HandlerFail("Trùng mã nhóm!");

            // Create project
            var project = new Project
            {
                TeamCode = newTeamCode,
                LeaderId = GetUserIdFromClaims(),
                TeamName = command.TeamName,
                TeamSize = command.TeamSize,
                SemesterId = semesterCurrent.Id,
                Status = ProjectStatus.Forming
            };
            await SetBaseEntityForCreation(project);
            _projectRepository.Add(project);

            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Đã xảy ra lỗi khi tạo nhóm thành công!");
            }

            // Create TeamMember
            var teamMember = new TeamMember
            {
                UserId = project.LeaderId,
                ProjectId = project.Id,
                Role = TeamMemberRole.Leader,
                JoinDate = DateTime.UtcNow,
                Status = TeamMemberStatus.Pending
            };
            await SetBaseEntityForCreation(teamMember);
            _teamMemberRepository.Add(teamMember);

            isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Tạo nhóm thành công!");
            }
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Đã xảy ra lỗi khi tạo nhóm");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while create {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> CreateProjectAndTeammemberForAuto(ProjectCreateCommand project)
    {
        try
        {
            // Create project
            var entity = await CreateOrUpdateEntity(project);
            var result = _mapper.Map<ProjectResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            // Create team
            var team = new TeamMemberCreateCommand
            {
                UserId = result.LeaderId,
                ProjectId = result.Id,
                Role = TeamMemberRole.Leader,
                JoinDate = DateTime.UtcNow,
                Status = TeamMemberStatus.InProgress
            };

            var teamresult = await _serviceTeam.CreateOrUpdate<TeamMemberResult>(team);
            if (teamresult.Status != 1) return teamresult;

            var msg = new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while create {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetProjectOfUserLogin()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var project = await _projectRepository.GetProjectOfUserLogin((Guid)userId);
                if (project != null && (project.Status == Domain.Enums.ProjectStatus.Pending || project.Status == ProjectStatus.Forming))
                {
                    if (project.LeaderId != userId)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage("Hiện tại người dùng không có quyền tuyển thành viên");
                    }

                    return new ResponseBuilder()
                        .WithData(project)
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_READ_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Hiện tại bạn không có project nào đủ điều kiện có thể tuyển thành viên");
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("User chưa đăng nhập");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public new async Task<BusinessResult> DeleteById(Guid id, bool isPermanent = false)
    {
        try
        {
            if (isPermanent)
            {
                // Delete Project
                var isDeleted = await DeleteEntityPermanently(id);
                return isDeleted
                    ? new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_DELETE_MSG)
                    : new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_DELETE_MSG);
            }

            var entity = await DeleteEntity(id);

            return entity != null
                ? new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG)
                : new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException?.Message.Contains("FOREIGN KEY") == true)
            {
                var errorMessage = "Không thể xóa vì dữ liệu đang được tham chiếu ở bảng khác.";
                return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(errorMessage)
                    ;
            }

            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while deleting {typeof(Project).Name} with ID {id}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> ExportExcelTeamsDefense(int defenseStage)
    {
        try
        {
            var teamDefense = await GetTeamsDefense(defenseStage);
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet(teamDefense, "Danh sách nhóm được ra bảo vệ");
                // Chèn dòng đầu tiên
                ws.Row(1).InsertRowsAbove(1); // Chèn một dòng mới phía trên

                // Gộp 6 cột đầu tiên
                ws.Range("A1:F1").Merge();

                // Gán tiêu đề
                ws.Cell("A1").Value = "LỊCH BẢO VỆ ĐỒ ÁN TỐT NGHIỆP";

                // Định dạng tiêu đề
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontSize = 14;
                ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Định dạng hàng tiêu đề (STT, Mã đề tài,...)
                var headerRow = ws.Range("A2:F2");
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray; // Màu nền tiêu đề

                // Căn giữa toàn bộ cột "STT"
                ws.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                int lastRow = ws.LastRowUsed().RowNumber();
                var range = ws.Range($"D3:D{lastRow}");

                range.CreateDataValidation()
                    .Date
                    .Between(DateTime.Now, new DateTime(9999, 12, 31));
                // Tự động điều chỉnh độ rộng cột
                ws.Columns().AdjustToContents();
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    byte[] fileBytes = ms.ToArray();
                    return new ResponseBuilder()
                        .WithData(fileBytes)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
                }
            }
        }

        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while export excel";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    private async Task<DataTable> GetTeamsDefense(int defenseStage)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("STT");
        dt.Columns.Add("Mã đề tài");
        dt.Columns.Add("Tên đề tài tiếng anh");
        dt.Columns.Add("Ngày");
        dt.Columns.Add("Thời gian");
        dt.Columns.Add("Hội trường");

        var currentSemester = await _semesterRepository.GetCurrentSemester();
        if (currentSemester == null)
        {
            return dt;
        }
        var teamDefenses = await _projectRepository.GetProjectBySemesterIdAndDefenseStage(currentSemester.Id, defenseStage);
        if (teamDefenses == null)
        {
            return dt;
        }
        if (teamDefenses.Count > 0)
        {
            int index = 1;
            teamDefenses.ForEach(item =>
            {
                //dt.Rows.Add(index++, item.Topic.TopicCode, item.Topic.EnglishName);
                dt.Rows.Add(index++, item.Topic.TopicCode, item.Topic.EnglishName);
            });
        }
        return dt;
    }

    public async Task<BusinessResult> UpdateDefenseStage(UpdateDefenseStage command)
    {
        try
        {
            // check date review 3
            var project = await _projectRepository.GetById(command.Id);
            if (project == null)
            {
                return new ResponseBuilder()
                     .WithStatus(Const.FAIL_CODE)
                     .WithMessage("Không tìm thấy dự án");
            }

            var review3 = await _reviewRepository.GetReviewByProjectIdAndNumber(project.Id, 3);
            if (review3 == null)
            {
                return new ResponseBuilder()
                     .WithStatus(Const.FAIL_CODE)
                     .WithMessage("Không tìm thấy review 3 của dự án");
            }

            var today = DateTime.UtcNow.Date;
            if (review3.ReviewDate.Value.Date > today)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Chưa đến ngày đánh giá");
            }

            if (today > review3.ReviewDate.Value.Date.AddDays(7)) return HandlerFail("Quá hạn đánh giá");
            //

            project.DefenseStage = command.DefenseStage;
            await SetBaseEntityForUpdate(project);
            _projectRepository.Update(project);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
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
            var errorMessage = $"An error {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
    public async Task<BusinessResult> GetProjectBySemesterAndStage(Guid semesterId, int stage)
    {
        try
        {
            var result = await _projectRepository.GetProjectBySemesterIdAndDefenseStage(semesterId, stage);
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithData(result)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> GetProjectNotInProgressYet()
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy kỳ");
            }
            var projectsWithMember = await _projectRepository.GetProjectNotInProgressYetInSemester(semester.Id);
            return new ResponseBuilder()
                .WithData(projectsWithMember)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> GetProjectNotCanceled()
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy kỳ");
            }
            var projectsWithMember = await _projectRepository.GetProjectNotCanceledInSemester(semester.Id);
            return new ResponseBuilder()
                .WithData(projectsWithMember)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> ManagerCreateProject(ProjectCreateByManagerCommand command)
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (command.TeamMembers.Count < semester.MinTeamSize || command.TeamMembers.Count > semester.MaxTeamSize)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Số lượng thành viên không phù hợp");
            }
            var topic = await _unitOfWork.TopicRepository.GetById(command.TopicId);
            if (topic == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy đề tài");
            }

            // var entity = _mapper.Map<Project>(command);
            var projects = await _projectRepository.GetInProgressProjectBySemesterId(semester.Id);
            var numberOfProjects = projects.Count();
            // Tạo số thứ tự tiếp theo
            int nextNumber = numberOfProjects + 1;
            string semesterCode = semester.SemesterCode;
            // Tạo mã nhóm
            string newTeamCode = $"{semesterCode}SE{nextNumber:D3}";

            var mapMember = _mapper.Map<List<TeamMember>>(command.TeamMembers);
            var entity = new Project
            {
                Id = Guid.NewGuid(),
                TeamCode = newTeamCode,
                LeaderId = command.LeaderId,
                TeamMembers = mapMember,
                Status = ProjectStatus.InProgress,
                TeamSize = command.TeamMembers.Count,
                SemesterId = semester.Id,
                TopicId = command.TopicId,
            };

            _projectRepository.Add(entity);

            topic.IsExistedTeam = true;
            _unitOfWork.TopicRepository.Update(topic);

            var saveChanges = await _unitOfWork.SaveChanges();
            if (!saveChanges)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            //notification to team members
            await _notificationService.CreateForTeam(new NotificationCreateForTeam
            {
                Description = "Bạn đã được thêm vào nhóm do quản lí xếp",
                ProjectId = entity.Id,
                Note = null
            });

            return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG)
                ;
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> SubmitBlockProjectByStudent(Guid projectId)
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy kỳ");
            }
            var project = await _projectRepository.GetById(projectId);
            if (project == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy nhóm");
            }
            if (project.TopicId == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Nhóm chưa có đề tài");
            }
            var members = await _teamMemberRepository.GetMembersOfTeamByProjectId(projectId);
            if (members.Count() < semester.MinTeamSize || members.Count() > semester.MaxTeamSize)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Số lượng thành viên của nhóm phải <= " + semester.MaxTeamSize + " và >= " + semester.MinTeamSize);
            }

            //chot nhom doi status sang Pending
            project.Status = ProjectStatus.Pending;
            await SetBaseEntityForUpdate(project);

            _projectRepository.Update(project);
            bool isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
            }
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> BlockProjectByManager(Guid projectId, BlockProjectByManager projectStatus)
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy kỳ");
            }
            var project = await _projectRepository.GetById(projectId);
            if (project == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Không tìm thấy nhóm");
            }
            if (projectStatus.status == ProjectStatus.InProgress)
            {
                if (project.TopicId == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhóm chưa có đề tài");
                }
                var members = await _teamMemberRepository.GetMembersOfTeamByProjectId(projectId);
                if (members.Count() < semester.MinTeamSize || members.Count() > semester.MaxTeamSize)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Số lượng thành viên của nhóm phải <= " + semester.MaxTeamSize + " và >= " + semester.MinTeamSize);
                }

                //chot nhom doi status sang InProgress va gen team code
                var teamCode = await _semesterService.GenerateNewTeamCode();
                if (teamCode == null)
                {
                    return new ResponseBuilder()
                   .WithStatus(Const.FAIL_CODE)
                   .WithMessage("Xảy ra lỗi khi gen team code");
                }
                project.TeamCode = teamCode;
                project.Status = ProjectStatus.InProgress;
            }
            else
            {
                project.Status = ProjectStatus.Forming;
            }
            
            await SetBaseEntityForUpdate(project);
            _projectRepository.Update(project);

            bool isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
            }
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> CancelProjectByManager(Guid projectId)
    {
        try
        {
            var project = await _projectRepository.GetById(projectId);
            if (project == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy nhóm");
            }

            var members = await _teamMemberRepository.GetMembersOfTeamByProjectId(projectId);
            if (project.TopicId != null)
            {
                var topic = await _topicRepository.GetTopicByProjectId(projectId);
                topic.IsExistedTeam = false;
                await SetBaseEntityForUpdate(topic);
                _topicRepository.Update(topic);
            }

            //update project
            project.Status = ProjectStatus.Canceled;
            project.TopicId = null;
            await SetBaseEntityForUpdate(project);
            _projectRepository.Update(project);

            //update members
            if (members != null)
            {
                foreach (var member in members)
                {
                    member.LeaveDate = DateTime.UtcNow;
                    await SetBaseEntityForUpdate(member);
                }

                _teamMemberRepository.UpdateRange(members);
            }

            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Xảy ra lỗi khi hủy nhóm");
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Hủy nhóm thành công");
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }
}