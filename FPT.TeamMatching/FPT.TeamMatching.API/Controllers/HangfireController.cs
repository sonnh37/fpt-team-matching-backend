using FPT.TeamMatching.Domain.Contracts.Hangfire;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.HANGFIRE)]
[ApiController]
public class HangfireController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IJobHangfireService _jobHangfireService;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IReviewService _reviewService;
    private readonly IIdeaService _ideaService;

    public HangfireController(IBackgroundJobClient backgroundJobClient, IJobHangfireService hangfireService,
        IRecurringJobManager recurringJobManager, IReviewService reviewService, IIdeaService ideaService)
    {
        _jobHangfireService = hangfireService;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _reviewService = reviewService;
        _ideaService = ideaService;
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
        _recurringJobManager.AddOrUpdate("public-idea-result", () => _ideaService.AutoUpdateIdeaStatus(), Cron.Minutely);
        return Ok();
    }

}