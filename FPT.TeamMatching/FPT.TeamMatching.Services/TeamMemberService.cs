﻿using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2016.Excel;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.Extensions.Logging.Abstractions;

namespace FPT.TeamMatching.Services;

public class TeamMemberService : BaseService<TeamMember>, ITeamMemberService
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMentorFeedbackRepository _mentorFeedbackRepository;
    private readonly IReviewRepository _reviewRepository;

    public TeamMemberService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _mentorFeedbackRepository = unitOfWork.MentorFeedbackRepository;
        _reviewRepository = unitOfWork.ReviewRepository;
    }

    public async Task<BusinessResult> GetTeamMemberByUserId()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFail("Không tìm thấy người dùng");
            var teamMemberCurrentUser = await _teamMemberRepository.GetMemberByUserId((Guid)userId.Value);
            if (teamMemberCurrentUser == null) return HandlerFail("Người dùng chưa có nhóm");

            return new ResponseBuilder()
                .WithData(teamMemberCurrentUser)
                .WithMessage(Const.SUCCESS_READ_MSG)
                .WithStatus(Const.SUCCESS_CODE);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> LeaveByCurrentUser()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFail("Không tìm thấy người dùng");
            var teamMemberCurrentUser = await _teamMemberRepository.GetTeamMemberActiveByUserId(userId.Value);
            if (teamMemberCurrentUser == null) return HandlerFail("Người dùng chưa có nhóm");

            teamMemberCurrentUser.LeaveDate = DateTime.UtcNow;
            teamMemberCurrentUser.IsDeleted = true;

            await SetBaseEntityForUpdate(teamMemberCurrentUser);
            _teamMemberRepository.DeletePermanently(teamMemberCurrentUser);

            var saveChanges = await _unitOfWork.SaveChanges();

            if (!saveChanges) return HandlerFail("Đã xảy ra lỗi khi lưu dữ liệu");

            return new ResponseBuilder()
                .WithData(teamMemberCurrentUser)
                .WithMessage(Const.SUCCESS_SAVE_MSG)
                .WithStatus(Const.SUCCESS_CODE);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> UpdateTeamMemberByManager(ManagerUpdate requests)
    {
        try
        {
            if (requests.updateList == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhập danh sách thành viên cần cập nhật");
            }

            var teamMember1 = await _teamMemberRepository.GetById(requests.updateList[0].Id);
            var project = await _projectRepository.GetById((Guid)teamMember1.ProjectId);
            if (requests.defenseNumber == 1)
            {
                //check status cua cac thanh vien
                var isStatusOfDefense2 = requests.updateList.Where(e =>
                        e.Status == Domain.Enums.TeamMemberStatus.Fail2 ||
                        e.Status == Domain.Enums.TeamMemberStatus.Pass2)
                    .Any();
                if (isStatusOfDefense2)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Status của các thành viên không được là status của defense 2");
                }

                //
                foreach (var request in requests.updateList)
                {
                    var teamMember = await _teamMemberRepository.GetById(request.Id);
                    if (teamMember == null)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.NOT_FOUND_CODE)
                            .WithMessage(Const.NOT_FOUND_MSG);
                    }

                    teamMember.Status = request.Status;
                    teamMember.CommentDefense1 = request.CommentDefense;
                    await SetBaseEntityForUpdate(teamMember);
                    _teamMemberRepository.Update(teamMember);
                }

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }

                if (project == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);
                }

                //check nếu team có fail1 thì đổi defenseStage sang 2
                var hasFail1 = requests.updateList.Any(e => e.Status == Domain.Enums.TeamMemberStatus.Fail1);
                if (hasFail1)
                {
                    project.DefenseStage = 2;
                    await SetBaseEntityForUpdate(project);
                    _projectRepository.Update(project);
                    var isSuccess2 = await _unitOfWork.SaveChanges();
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

                //check all pass
                var allPassed1 = requests.updateList.All(e => e.Status == Domain.Enums.TeamMemberStatus.Pass1);
                if (allPassed1)
                {
                    project.Status = Domain.Enums.ProjectStatus.Completed;
                    await SetBaseEntityForUpdate(project);
                    _projectRepository.Update(project);

                    isSuccess = await _unitOfWork.SaveChanges();
                    if (!isSuccess)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG);
                    }
                }
            }
            else if (requests.defenseNumber == 2)
            {
                //check status cua cac thanh vien
                var isStatusOfDefense1 = requests.updateList.Where(e =>
                        e.Status == Domain.Enums.TeamMemberStatus.Fail1 ||
                        e.Status == Domain.Enums.TeamMemberStatus.Pass1)
                    .Any();
                if (isStatusOfDefense1)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Status của các thành viên không được là status của defense 1");
                }

                foreach (var request in requests.updateList)
                {
                    var teamMember = await _teamMemberRepository.GetById(request.Id);
                    if (teamMember == null)
                    {
                        return new ResponseBuilder()
                            .WithStatus(Const.NOT_FOUND_CODE)
                            .WithMessage(Const.NOT_FOUND_MSG);
                    }

                    teamMember.Status = request.Status;
                    teamMember.CommentDefense2 = request.CommentDefense;
                    await SetBaseEntityForUpdate(teamMember);
                    _teamMemberRepository.Update(teamMember);
                }

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }

                project.Status = Domain.Enums.ProjectStatus.Completed;
                await SetBaseEntityForUpdate(project);
                _projectRepository.Update(project);

                isSuccess = await _unitOfWork.SaveChanges();
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
            else
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Lần bảo vệ đồ án phải là 1 hoặc 2");
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

    public async Task<BusinessResult> UpdateTeamMemberByMentor(List<MentorUpdate> requests)
    {
        try
        {
            // check date review 3
            var tm = await _teamMemberRepository.GetById(requests[0].Id);
            if (tm == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage("Không tìm thấy thành viên");
            }

            var project = await _projectRepository.GetById(tm.ProjectId);
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

            foreach (var request in requests)
            {
                var teamMember = await _teamMemberRepository.GetById(request.Id);
                if (teamMember == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Không tìm thấy thành viên");
                }

                teamMember.MentorConclusion = request.MentorConclusion;
                teamMember.Attitude = request.Attitude;
                teamMember.Note = request.Note;
                await SetBaseEntityForUpdate(teamMember);
                _teamMemberRepository.Update(teamMember);
            }

            var isSuccess = await _unitOfWork.SaveChanges();
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
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }
}