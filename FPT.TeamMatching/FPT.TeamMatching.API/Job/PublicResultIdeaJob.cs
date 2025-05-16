using FPT.TeamMatching.Domain.Contracts.Services;
using Quartz;

namespace FPT.TeamMatching.API.Job
{
    public class PublicResultIdeaJob: IJob
    {
        private readonly ITopicService _topicService;

        public PublicResultIdeaJob(ITopicService topicService, ITopicRequestService topicVersionRequestService, IProjectService projectService)
        {
            _topicService = topicService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _topicService.AutoUpdateTopicStatus();
        }
    }
}
