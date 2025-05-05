using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_MENTOR_FEEDBACKS)]
    [ApiController]
    [Authorize]
    public class MentorFeedbackController : ControllerBase
    {
        private readonly IMentorFeedbackService _service;

        public MentorFeedbackController(IMentorFeedbackService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MentorFeedbackGetAllQuery query)
        {
            var msg = await _service.GetAll<MentorFeedbackResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<MentorFeedbackResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MentorFeedbackCreateCommand request)
        {
            var msg = await _service.CreateMentorFeedbackAfterReview3(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MentorFeedbackUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<MentorFeedbackResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] MentorFeedbackRestoreCommand command)
        {
            var businessResult = await _service.Restore<MentorFeedbackResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] MentorFeedbackDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

    }
}
