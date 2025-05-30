using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Requests.Commands.StageTopics;

namespace FPT.TeamMatching.Services
{
    public class StageTopicService : BaseService<StageTopic>, IStageTopicService
    {
        private readonly IStageTopicRepository _stageTopicRepositoty;
        private readonly ITopicRepository _topicRepository;

        public StageTopicService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _stageTopicRepositoty = unitOfWork.StageTopicRepository;
            _topicRepository = unitOfWork.TopicRepository;
        }

        public async Task<BusinessResult> Create(StageTopicCreateCommand command)
        {
            try
            {
                // check semester
                var semester = await GetSemesterInCurrentWorkSpace();
                if (semester == null)
                {
                    return HandlerFail("Không tìm thấy kỳ");
                }
                if (semester.Status != SemesterStatus.Preparing)
                {
                    return HandlerFail("Hiện tại không được tạo đợt duyệt");
                }

                //check start date sau start date semester
                if (command.StartDate <= semester.StartDate)
                {
                    return HandlerFail("Ngày bắt đầu của đợt duyệt phải sau ngày bắt đầu của học kỳ");
                }

                //check end date trc ongoing date cua semester
                if (command.EndDate >= semester.OnGoingDate)
                {
                    return HandlerFail("Ngày kết thúc của đợt duyệt phải trước ngày khóa nhóm của học kỳ");
                }

                //check end date sau start date 
                if (command.StartDate >= command.EndDate)
                {
                    return HandlerFail("Ngày bắt đầu của đợt duyệt phải trước ngày kết thúc");
                }

                //check result date sau end date stage topic
                if (command.ResultDate <= command.EndDate)
                {
                    return HandlerFail("Ngày công bố kết quả đợt duyệt phải sau ngày kết thúc đợt duyệt");
                }
                //check result date truoc end date semester
                if (command.ResultDate >= semester.OnGoingDate)
                {
                    return HandlerFail("Ngày công bố kết quả đợt duyệt phải trước ngày khóa nhóm của học kỳ");
                }

                var stageTopic = _mapper.Map<StageTopic>(command);
                //tự tăng StageNumber
                var stageTopics = await _stageTopicRepositoty.GetStageTopicsOfSemester(semester.Id);
                stageTopic.StageNumber = stageTopics.Count() + 1;
                stageTopic.SemesterId = semester.Id;
                await SetBaseEntityForCreation(stageTopic);
                _stageTopicRepositoty.Add(stageTopic);

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return HandlerFail("Đã xảy ra lỗi khi tạo đợt duyệt");
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Tạo đợt duyệt thành công");
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(StageTopicResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetByStageNumber<TResult>(int number) where TResult : BaseResult
        {
            try
            {
                var semester = await _unitOfWork.SemesterRepository.GetUpComingSemester();
                if (semester == null) return HandlerFail("Không tìm thấy kỳ");

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

        public async Task<BusinessResult> ShowResult(Guid stageTopicId)
        {
            try
            {
                var stageTopic = await _stageTopicRepositoty.GetById(stageTopicId);
                if (stageTopic == null) return HandlerFail("Không tìm thấy đợt duyệt");

                var pendingTopic = await _topicRepository.GetTopicWithStatusInStageTopic([TopicStatus.ManagerPending], stageTopicId);

                if (pendingTopic.Count() != 0)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Còn " + pendingTopic.Count() + " đề tài chưa được duyệt");
                }

                stageTopic.ResultDate = DateTime.UtcNow;
                await SetBaseEntityForUpdate(stageTopic);

                _stageTopicRepositoty.Update(stageTopic);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Đã xảy ra lỗi khi cập nhật đợt duyệt");
                }
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Đã công bố kết quả đợt duyệt");
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(StageTopicResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> Update(StageTopicUpdateCommand command)
        {
            try
            {
                var stageTopic = await _stageTopicRepositoty.GetById(command.Id);
                if (stageTopic == null)
                {
                    return HandlerFail("Không tìm thấy đợt duyệt");
                }

                // check semester
                var semester = await GetSemesterInCurrentWorkSpace();
                if (semester == null)
                {
                    return HandlerFail("Không tìm thấy kỳ");
                }
                if (semester.Status != SemesterStatus.Preparing)
                {
                    return HandlerFail("Hiện tại không được cập nhật đợt duyệt");
                }

                //check start date sau start date semester
                if (command.StartDate <= semester.StartDate)
                {
                    return HandlerFail("Ngày bắt đầu của đợt duyệt phải sau ngày bắt đầu của học kỳ");
                }

                //check end date trc ongoing date cua semester
                if (command.EndDate >= semester.OnGoingDate)
                {
                    return HandlerFail("Ngày kết thúc của đợt duyệt phải trước ngày khóa nhóm của học kỳ");
                }

                //check end date sau start date 
                if (command.StartDate >= command.EndDate)
                {
                    return HandlerFail("Ngày bắt đầu của đợt duyệt phải trước ngày kết thúc");
                }

                //check result date sau end date stage topic
                if (command.ResultDate <= command.EndDate)
                {
                    return HandlerFail("Ngày công bố kết quả đợt duyệt phải sau ngày kết thúc đợt duyệt");
                }

                //check result date truoc end date semester
                if (command.ResultDate >= semester.OnGoingDate)
                {
                    return HandlerFail("Ngày công bố kết quả đợt duyệt phải trước ngày khóa nhóm của học kỳ");
                }

                stageTopic.StartDate = (DateTimeOffset)command.StartDate;
                stageTopic.EndDate = (DateTimeOffset)command.EndDate;
                stageTopic.ResultDate = (DateTimeOffset)command.ResultDate;

                await SetBaseEntityForUpdate(stageTopic);
                _stageTopicRepositoty.Update(stageTopic);

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return HandlerFail("Đã xảy ra lỗi khi cập nhật đợt duyệt");
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Cập nhật đợt duyệt thành công");
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(StageTopicResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}