﻿using FPT.TeamMatching.Domain.Contracts.Hangfire;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ITopicService _topicService;
    private readonly IConfiguration _configuration;

    public HangfireController(IBackgroundJobClient backgroundJobClient, IJobHangfireService hangfireService,
        IRecurringJobManager recurringJobManager, IReviewService reviewService, ITopicService topicService, IConfiguration configuration)
    {
        _jobHangfireService = hangfireService;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _reviewService = reviewService;
        _topicService = topicService;
        _configuration = configuration;
    }

    [HttpGet("/FireAndForgetJob")]
    public ActionResult CreateFireAndForgetJob()
    {
        _backgroundJobClient.Enqueue(() => _jobHangfireService.FireAndForgetJob());
        return Ok();
    }

    [HttpGet("/RecurringJob/save-chat")]
    public ActionResult CreateRecurringJob()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        _recurringJobManager.AddOrUpdate("save-chat", () => _jobHangfireService.ReccuringJob(), "0 3 * * *", timeZone);
        return Ok();
    }

    [HttpGet("/RecurringJob/create-reviews")]
    public ActionResult CreateReviews()
    {
        _recurringJobManager.AddOrUpdate("create-reviews", () => _reviewService.CreateReviewsForActiveProject(), Cron.Daily);
        return Ok();
    }

    [HttpGet("background-job/public-topic-result")]
    public ActionResult PublicTopicResult()
    {
        var name = _configuration.GetSection("HANGFIRE_SERVER_LOCAL");
        var selectedDate = DateTimeOffset.Parse("2025-4-29 00:00:00");
        _recurringJobManager.RemoveIfExists("public-topic-result");
        _recurringJobManager.AddOrUpdate("public-topic-result", () => _topicService.AutoUpdateTopicStatus(), "0 0 30 5 *", new RecurringJobOptions { QueueName = name.Value });
        _backgroundJobClient.Schedule(() => _topicService.AutoUpdateTopicStatus(), selectedDate);
        return Ok();
    }

    [HttpGet("/RecurringJob/update-when-semester-start")]
    public ActionResult UpdateProjectInProgress()
    {
        var name = _configuration.GetSection("HANGFIRE_SERVER_LOCAL");
        _recurringJobManager.AddOrUpdate("update-when-semester-start", () => _topicService.UpdateWhenSemesterStart(), Cron.Minutely, new RecurringJobOptions { QueueName = name.Value });
        return Ok();
    }

    [HttpGet("trigger-now")]
    public ActionResult TriggerNow([FromQuery] string jobId)
    {
        _recurringJobManager.Trigger(jobId);
        // _recurringJobManager.RemoveIfExists(jobId);
        return Ok($"Triggered {jobId} job successfully!");
    }
}