using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services
{
    public class StageTopicService : BaseService<StageTopic>, IStageTopicService
    {
        private readonly IStageTopicRepository _stageTopicRepositoty;

        public StageTopicService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _stageTopicRepositoty = unitOfWork.StageTopicRepository;
        }

        public async Task<BusinessResult> GetByStageNumber<TResult>(int number) where TResult : BaseResult
        {
            try
            {
                var semester = await _unitOfWork.SemesterRepository.GetUpComingSemester();
                if (semester == null) return HandlerFail("Không tìm thấy kì");

                var entity = await _stageTopicRepositoty.GetByStageNumberAndSemester(number, semester.Id);
                var result = _mapper.Map<TResult>(entity);
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
                var errorMessage = $"An error {typeof(TResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
        
        public async Task<BusinessResult> GetCurrentStageTopic<TResult>() where TResult : BaseResult
        {
            try
            {
                var entity = await _stageTopicRepositoty.GetCurrentStageTopic();
                var result = _mapper.Map<TResult>(entity);
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
                var errorMessage = $"An error {typeof(TResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}