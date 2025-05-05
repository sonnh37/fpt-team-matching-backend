using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services
{
    public class SemesterService : BaseService<Semester>, ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        public SemesterService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _semesterRepository = unitOfWork.SemesterRepository;
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
        
        public async Task<string> GenerateNewTopicCode(Guid? semesterId)
        {
            try
            {
                var semester = await _semesterRepository.GetById(semesterId);
                if (semester == null) return string.Empty;
                var semesterCode = semester.SemesterCode;
                var semesterPrefix = semester.SemesterPrefixName;
                    
                //get so luong idea dc duyet approve cua ki
                var numberOfTopics = _unitOfWork.TopicRepository.NumberOfTopicBySemesterId(semester.Id);

                // Tạo số thứ tự tiếp theo
                int nextNumberTopic = numberOfTopics + 1;

                // Tạo mã Idea mới theo định dạng:
                // semesterPrefix + semesterCode + "SE" + số thứ tự (2 chữ số)
                string newTopicCode = $"{semesterPrefix}{semesterCode}SE{nextNumberTopic:D2}";
                return newTopicCode;
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(SemesterResult).Name}: {ex.Message}";
                return string.Empty;
            }
        }
        
        public async Task<string> GenerateNewTeamCode(Guid? semesterId)
        {
            try
            {
                var semester = await _semesterRepository.GetById(semesterId);
                if (semester == null) return string.Empty;
                var numberOfProjects = await _unitOfWork.ProjectRepository.NumberOfInProgressProjectInSemester(semester.Id);
                // Tạo số thứ tự tiếp theo
                int nextNumber = numberOfProjects + 1;
                string semesterCode = semester.SemesterCode;
                //Tạo mã nhóm
                string newTeamCode = $"{semesterCode}SE{nextNumber:D3}";

                return newTeamCode;

            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(SemesterResult).Name}: {ex.Message}";
                return string.Empty;
            }
        }

        // public async Task<BusinessResult> GetPresentSemester()
        // {
        //     try
        //     {
        //         var s = await _semesterRepository.GetPresentSemester();
        //         if (s == null)
        //         {
        //             return new ResponseBuilder()
        //             .WithStatus(Const.NOT_FOUND_CODE)
        //             .WithMessage(Const.NOT_FOUND_MSG);
        //         }
        //         return new ResponseBuilder()
        //             .WithData(s)
        //             .WithStatus(Const.SUCCESS_CODE)
        //             .WithMessage(Const.SUCCESS_READ_MSG);
        //     }
        //     catch (Exception ex)
        //     {
        //         var errorMessage = $"An error {typeof(SemesterResult).Name}: {ex.Message}";
        //         return new ResponseBuilder()
        //             .WithStatus(Const.FAIL_CODE)
        //             .WithMessage(errorMessage);
        //     }
        // }

    }
}
