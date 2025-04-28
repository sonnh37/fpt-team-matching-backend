using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services
{
    public class TopicService : BaseService<Topic>, ITopicService
    {
        public TopicService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
        }

        public async Task<BusinessResult> GetTopicsForMentor(TopicGetListForMentorQuery query)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId == null) return HandlerFailAuth();

                var (data, total) = await _unitOfWork.TopicRepository.GetTopicsForMentor(query, userId.Value);
                var results = _mapper.Map<List<TopicResult>>(data);
                var response = new QueryResult(query, results, total);

                return new ResponseBuilder()
                    .WithData(response)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(TopicResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
