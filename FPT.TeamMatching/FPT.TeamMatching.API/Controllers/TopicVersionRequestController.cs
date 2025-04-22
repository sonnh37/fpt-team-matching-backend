using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicVersionRequests;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_TOPIC_VERSION_REQUESTS)]
    [ApiController]
    public class TopicVersionRequestController : ControllerBase
    {
        private readonly ITopicVersionRequestService _service;

        public TopicVersionRequestController(ITopicVersionRequestService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TopicVersionRequestGetAllQuery query)
        {
            var msg = await _service.GetAll<TopicVersionRequestResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<TopicVersionRequestResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TopicVersionRequestCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<TopicVersionRequestResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TopicVersionRequestUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<TopicVersionRequestResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] TopicVersionRequestRestoreCommand command)
        {
            var businessResult = await _service.Restore<TopicVersionRequestResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] TopicVersionRequestDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("respond-by-mentor-or-manager")]
        public async Task<IActionResult> RespondByMentorOrManager([FromBody] RespondByMentorOrManager request)
        {
            var businessResult = await _service.RespondByLecturerOrManager(request);
            return Ok(businessResult);
        }

        [HttpGet("get-by-role")]
        public async Task<IActionResult> GetByRole([FromQuery] string role)
        {
            var msg = await _service.GetByRole(role);
            return Ok(msg);
        }
    }
}
