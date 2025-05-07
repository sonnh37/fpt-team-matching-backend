using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Semester;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Semester;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_SEMESTERS)]
    [ApiController]
    [Authorize]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _service;
        private readonly IConfiguration _configuration;
        private readonly IIdeaService _ideaService;
        private readonly IReviewService _reviewService;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public SemesterController(
            ISemesterService __service,
            IConfiguration configuration,
            IIdeaService ideaService,
            IReviewService reviewService,
            IRecurringJobManager recurringJobManager,
            IBackgroundJobClient backgroundJobClient
            )
        {
            _service = __service;
            _configuration = configuration;
            _ideaService = ideaService;
            _reviewService = reviewService;
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SemesterGetAllQuery query)
        {
            var msg = await _service.GetAll<SemesterResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<SemesterResult>(id);
            return Ok(msg);
        }
        
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSemester()
        {
            var msg = await _service.GetCurrentSemester();
            return Ok(msg);
        }
        
        [HttpGet("before")]
        public async Task<IActionResult> GetBeforeSemester()
        {
            var msg = await _service.GetBeforeSemester();
            return Ok(msg);
        }
        
        [HttpGet("up-coming")]
        public async Task<IActionResult> GetUpcomingSemester()
        {
            var msg = await _service.GetUpComingSemester();
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SemesterCreateCommand request)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var msg = await _service.CreateOrUpdate<SemesterResult>(request);
            if (msg.Status == 1)
            {
                // auto-update-when-semester-start
                // var name = _configuration.GetSection("HANGFIRE_SERVER_LOCAL");
                var timeUpdateProject = Utils.ToCronExpression(request.StartDate.Value);
                _recurringJobManager.AddOrUpdate("auto-update-project-inprogress-"+request.SemesterCode, () => _ideaService.UpdateWhenSemesterStart(), timeUpdateProject, new RecurringJobOptions { TimeZone = timeZone});
                // create review hangfire
                var timeCreateReview = Utils.ToCronExpression(request.StartDate.Value); // deplay for 5 minutes for project updated
                _recurringJobManager.AddOrUpdate("auto-create-review-"+request.SemesterCode, () => _reviewService.CreateReviewsForActiveProject(),timeCreateReview , new RecurringJobOptions { TimeZone = timeZone });
                
                // auto update idea status 
                var timePublicIdeaResult = Utils.ToCronExpression(request.PublicTopicDate.Value);
                _recurringJobManager.AddOrUpdate("auto-update-result-"+request.SemesterCode, () => _ideaService.AutoUpdateIdeaStatus(), timePublicIdeaResult, new RecurringJobOptions { TimeZone = timeZone });
                
            }
          
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SemesterUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<SemesterResult>(request);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            // var name = _configuration.GetSection("HANGFIRE_SERVER_LOCAL");
           
            var timeUpdateProject = Utils.ToCronExpression(request.StartDate.Value.LocalDateTime);
            _recurringJobManager.AddOrUpdate("auto-update-project-inprogress-"+request.SemesterCode, () => _ideaService.UpdateWhenSemesterStart(), timeUpdateProject, new RecurringJobOptions { TimeZone = timeZone});
            // create review hangfire
            // _backgroundJobClient.Schedule(() => _ideaService.UpdateWhenSemesterStart(),
            //     request.StartDate.Value.LocalDateTime.AddHours(23) - DateTimeOffset.Now);
           
            var timeCreateReview = Utils.ToCronExpression(request.StartDate.Value); // deplay for 5 minutes for project updated
            _recurringJobManager.AddOrUpdate("auto-create-review-"+request.SemesterCode, () => _reviewService.CreateReviewsForActiveProject(),timeCreateReview , new RecurringJobOptions { TimeZone = timeZone });
           
            var timePublicIdeaResult = Utils.ToCronExpression(request.PublicTopicDate.Value);
            _recurringJobManager.AddOrUpdate("auto-update-result-"+request.SemesterCode, () => _ideaService.AutoUpdateIdeaStatus(), timePublicIdeaResult, new RecurringJobOptions { TimeZone = timeZone });
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] SemesterRestoreCommand command)
        {
            var businessResult = await _service.Restore<SemesterResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] SemesterDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        // [HttpGet("get-present-semester")]
        // public async Task<IActionResult> GetPresentSemester()
        // {
        //     var msg = await _service.GetPresentSemester();
        //     return Ok(msg);
        // }
    }
}
