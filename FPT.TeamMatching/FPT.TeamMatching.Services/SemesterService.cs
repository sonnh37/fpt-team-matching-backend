﻿using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services
{
    public class SemesterService : BaseService<Semester>, ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        public SemesterService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _semesterRepository = unitOfWork.SemesterRepository;
            _topicRepository = unitOfWork.TopicRepository;
            _projectRepository = unitOfWork.ProjectRepository;
            _userRepository = unitOfWork.UserRepository;
        }

        public async Task<BusinessResult> GetCurrentSemester()
        {
            try
            {
                var entity = await _semesterRepository.GetCurrentSemester();
                var result = _mapper.Map<SemesterResult>(entity);
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
                var errorMessage = $"An error {typeof(SemesterResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetBeforeSemester()
        {
            try
            {
                var entity = await _semesterRepository.GetBeforeSemester();
                var result = _mapper.Map<SemesterResult>(entity);
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
                var errorMessage = $"An error {typeof(SemesterResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetUpComingSemester()
        {
            try
            {
                var entity = await _semesterRepository.GetUpComingSemester();
                var result = _mapper.Map<SemesterResult>(entity);
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
                var errorMessage = $"An error {typeof(SemesterResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<string> GenerateNewTopicCode()
        {
            try
            {
                var semester = await GetSemesterInCurrentWorkSpace();
                if (semester == null) return string.Empty;
                var semesterCode = semester.SemesterCode;
                var semesterPrefix = semester.SemesterPrefixName;

                //get so luong topic dc duyet approve cua ki
                var topics = await _topicRepository.ApprovedTopicsBySemesterId(semester.Id);
                var numberOfTopics = topics.Count();
                int nextNumber = numberOfTopics;
                string newTopicCode = "";

                //check trùng mã
                bool isExisted = true;
                do
                {
                    // Tạo số thứ tự tiếp theo
                    nextNumber += 1;
                    // Tạo mã Topic mới theo định dạng:
                    // semesterPrefix + semesterCode + "SE" + số thứ tự (2 chữ số)
                    newTopicCode = $"{semesterPrefix}{semesterCode}SE{nextNumber:D2}";
                    isExisted = await _topicRepository.IsExistTopicCode(newTopicCode);
                }
                while (isExisted);

                return newTopicCode;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> GenerateNewTeamCode()
        {
            try
            {
                var semester = await GetSemesterInCurrentWorkSpace();
                if (semester == null) return string.Empty;
                var projects = await _projectRepository.GetInProgressProjectBySemesterId(semester.Id);
                var numberOfProjects = projects.Count();
                int nextNumber = numberOfProjects;
                string semesterCode = semester.SemesterCode;
                var newTeamCode = "";

                //check trùng mã
                bool isExisted = true;
                do
                {
                    // Tạo số thứ tự tiếp theo
                    nextNumber += 1;
                    // Tạo mã nhóm
                    newTeamCode = $"{semesterCode}SE{nextNumber:D3}";
                    isExisted = await _projectRepository.IsExistedTeamCode(newTeamCode);
                }
                while (isExisted);

                return newTeamCode;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<BusinessResult> UpdateStatus(SemesterStatus status)
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy kì");
            }
            //status phai la preparing, ongoing, closed
            if (status == SemesterStatus.NotStarted)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Trạng thái tiếp theo của kì không thể là Not Started");
            }
            //status = Preparing
            if (status == SemesterStatus.Preparing)
            {
                semester.StartDate = DateTime.UtcNow;

            }

            //status = OnGoing
            else if (status == SemesterStatus.OnGoing)
            {
                //check xem còn nhóm nào đang pending k
                var projects = await _projectRepository.GetProjectNotInProgressYetInSemester(semester.Id);
                if (projects.Count() != 0)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Còn " + projects.Count() + " nhóm chưa được chốt");
                }

                //check xem còn student nào chưa có nhóm k
                var students = await _userRepository.GetStudentDoNotHaveTeam(semester.Id);
                if (students != null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Còn " + students.Count() + " sinh viên chưa có nhóm");
                }

                semester.OnGoingDate = DateTime.UtcNow;
            }

            //status = Closed
            else if (status == SemesterStatus.Closed)
            {
                var students = await _userRepository.GetStudentDoNotHaveFinalResult(semester.Id);
                if (students.Count() != 0)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Còn " + students.Count() + " sinh viên chưa được cập nhật kết quả");
                }
                semester.EndDate = DateTime.UtcNow;
            }

            //update status semester
            semester.Status = status;
            await SetBaseEntityForUpdate(semester);
            _semesterRepository.Update(semester);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Đã xảy ra lỗi khi cập nhật học kì");
            }
            return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Chuyển trạng thái học kì sang " + status.ToString() + " thành công");
        }
    }
}
