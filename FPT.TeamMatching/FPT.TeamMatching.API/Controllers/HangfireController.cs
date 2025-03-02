using FPT.TeamMatching.Domain.Contracts.Hangfire;
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

    public HangfireController(IBackgroundJobClient backgroundJobClient, IJobHangfireService hangfireService,
        IRecurringJobManager recurringJobManager)
    {
        _jobHangfireService = hangfireService;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
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
}