using FPT.TeamMatching.Domain.Contracts.Services;
using Quartz;

namespace FPT.TeamMatching.API.Job
{
    public class ReviewCreationJob : IJob
    {
        private readonly IReviewService _reviewService;

        public ReviewCreationJob(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _reviewService.CreateReviewsForActiveProject();
        }
    }
}
