using FPT.TeamMatching.Domain.Contracts.Hangfire;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.HANGFIRE)]
[ApiController]
[Authorize]
public class HangfireController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IJobHangfireService _jobHangfireService;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IReviewService _reviewService;
    private readonly IIdeaService _ideaService;
    private readonly IConfiguration _configuration;

    public HangfireController(IBackgroundJobClient backgroundJobClient, IJobHangfireService hangfireService,
        IRecurringJobManager recurringJobManager, IReviewService reviewService, IIdeaService ideaService, IConfiguration configuration)
    {
        _jobHangfireService = hangfireService;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _reviewService = reviewService;
        _ideaService = ideaService;
        _configuration = configuration;
    }

    [HttpGet("/FireAndForgetJob")]
    public ActionResult CreateFireAndForgetJob()
    {
        _backgroundJobClient.Enqueue(() => _jobHangfireService.FireAndForgetJob());
        return Ok();
    }

    [HttpGet("/RecurringJob/${jobId:int}")]
    public ActionResult CreateRecurringJob(int jobId)
    {
        _recurringJobManager.AddOrUpdate(jobId.ToString(), () => _jobHangfireService.ReccuringJob(), Cron.Minutely);
        return Ok();
    }

    [HttpGet("/RecurringJob/create-reviews")]
    public ActionResult CreateReviews()
    {
        _recurringJobManager.AddOrUpdate("create-reviews", () => _reviewService.CreateReviewsForActiveProject(),Cron.Minutely);
        return Ok();
    }

    [HttpGet("/RecurringJob/public-idea-result")]
    public ActionResult PublicIdeaResult()
    {
        var name = _configuration.GetSection("HANGFIRE_SERVER_LOCAL");
        _recurringJobManager.AddOrUpdate("public-idea-result", () => _ideaService.AutoUpdateIdeaStatus(), Cron.Daily, new RecurringJobOptions { QueueName = name.Value });
        return Ok();
    }

    [HttpGet("/RecurringJob/update-project-in-progress")]
    public ActionResult UpdateProjectInProgress()
    {
        var name = _configuration.GetSection("HANGFIRE_SERVER_LOCAL");
        _recurringJobManager.AddOrUpdate("update-project-in-progress", () => _ideaService.AutoUpdateProjectInProgress(), Cron.Minutely, new RecurringJobOptions { QueueName = name.Value });
        return Ok();
    }
}