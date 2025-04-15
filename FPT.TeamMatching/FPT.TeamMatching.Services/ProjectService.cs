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
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

// ReSharper disable All

namespace FPT.TeamMatching.Services;

public class ProjectService : BaseService<Project>, IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamMemberService _serviceTeam;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly ISemesterRepository _semesterRepository;

    public ProjectService(IMapper mapper, IUnitOfWork unitOfWork, ITeamMemberService teamMemberService) : base(mapper,
        unitOfWork)
    {
        _projectRepository = unitOfWork.ProjectRepository;
        _serviceTeam = teamMemberService;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
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

    public async Task<BusinessResult> GetProjectByUserIdLogin()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFailAuth();

            var project = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
            if (project == null) return HandlerFail("Người dùng không có project đang tồn tại");

            if (project.Status == ProjectStatus.Canceled ||
                project.Status == ProjectStatus.Completed)
                return HandlerFail("Người dùng không có project đang tồn tại trong kì này");

            var result = _mapper.Map<ProjectResult>(project);
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
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> CreateProjectAndTeammember(TeamCreateCommand project)
    {
        try
        {
            // Create project
            var command = new ProjectCreateCommand
            {
                LeaderId = GetUserIdFromClaims(),
                TeamName = project.TeamName,
                TeamSize = project.TeamSize,
                Status = ProjectStatus.Pending
            };
            var entity = await CreateOrUpdateEntity(command);
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
                Role = Domain.Enums.TeamMemberRole.Leader,
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
                if (project != null && project.Status == Domain.Enums.ProjectStatus.Pending)
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
                //sua db
                //dt.Rows.Add(index++, item.Idea.IdeaCode, item.Idea.EnglishName);
                dt.Rows.Add(index++, item.Topic.TopicCode, item.Topic.Idea.EnglishName);
            });
        }
        return dt;
    }

    public async Task<BusinessResult> UpdateDefenStage(UpdateDefenseStage command)
    {
        try
        {
            var project = await _projectRepository.GetById(command.Id);
            if (project == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage(Const.NOT_FOUND_MSG);
            }
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
}