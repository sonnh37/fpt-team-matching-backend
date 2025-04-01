using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_MENTOR_FEEDBACKS)]
    [ApiController]
    public class MentorFeedbackController : ControllerBase
    {
        private readonly IMentorFeedbackService _likeservice;

        public MentorFeedbackController(IMentorFeedbackService _service)
        {
            _likeservice = _service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MentorFeedbackGetAllQuery query)
        {
            var msg = await _likeservice.GetAll<MentorFeedbackResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _likeservice.GetById<MentorFeedbackResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MentorFeedbackCreateCommand request)
        {
            var msg = await _likeservice.CreateOrUpdate<MentorFeedbackResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MentorFeedbackUpdateCommand request)
        {
            var businessResult = await _likeservice.CreateOrUpdate<MentorFeedbackResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] MentorFeedbackRestoreCommand command)
        {
            var businessResult = await _likeservice.Restore<MentorFeedbackResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] MentorFeedbackDeleteCommand request)
        {
            var businessResult = await _likeservice.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

    }
}
