using FPT.TeamMatching.Domain.Contracts.Services;
using Quartz;

namespace FPT.TeamMatching.API.Job
{
    public class PublicResultIdeaJob: IJob
    {
        private readonly IIdeaService _ideaService;
        private readonly IIdeaRequestService _ideaRequestService;
        private readonly IProjectService _projectService;

        public PublicResultIdeaJob(IIdeaService ideaService, IIdeaRequestService ideaRequestService, IProjectService projectService)
        {
            _ideaService = ideaService;
            _ideaRequestService = ideaRequestService;
            _projectService = projectService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //update idea status

            //idea status == approve => tao project

        }
    }
}
