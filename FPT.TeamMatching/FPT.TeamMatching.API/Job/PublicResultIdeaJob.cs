using FPT.TeamMatching.Domain.Contracts.Services;
using Quartz;

namespace FPT.TeamMatching.API.Job
{
    public class PublicResultIdeaJob: IJob
    {
        private readonly ITopicService _ideaService;

        public PublicResultIdeaJob(ITopicService ideaService, ITopicRequestService ideaVersionRequestService, IProjectService projectService)
        {
            _ideaService = ideaService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //sua db
            //await _ideaService.AutoUpdateIdeaStatus();
        }
    }
}
