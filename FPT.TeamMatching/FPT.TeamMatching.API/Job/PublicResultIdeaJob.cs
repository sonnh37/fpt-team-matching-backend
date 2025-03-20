using FPT.TeamMatching.Domain.Contracts.Services;
using Quartz;

namespace FPT.TeamMatching.API.Job
{
    public class PublicResultIdeaJob: IJob
    {
        private readonly IIdeaService _ideaService;

        public PublicResultIdeaJob(IIdeaService ideaService, IIdeaRequestService ideaRequestService, IProjectService projectService)
        {
            _ideaService = ideaService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _ideaService.AutoUpdateIdeaStatus();
        }
    }
}
