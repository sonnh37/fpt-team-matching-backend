using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorTopicRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorTopicRequests;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_MENTOR_TOPIC_REQUESTS)]
    [ApiController]
    [Authorize]
    public class MentorTopicRequestController : ControllerBase
    {
        private readonly IMentorTopicRequestService _service;

        public MentorTopicRequestController(IMentorTopicRequestService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MentorTopicRequestGetAllQuery query)
        {
            var msg = await _service.GetAll<MentorTopicRequestResult>(query);
            return Ok(msg);
        }
        
        [HttpGet("get-user-mentor-idea-requests")]
        public async Task<IActionResult> GetUserMentorTopicRequests([FromQuery] MentorTopicRequestGetAllQuery query)
        {
            var msg = await _service.GetUserMentorTopicRequests(query);
            return Ok(msg);
        }
        
        [HttpGet("get-mentor-mentor-idea-requests")]
        public async Task<IActionResult> GetMentorMentorTopicRequests([FromQuery] MentorTopicRequestGetAllQuery query)
        {
            var msg = await _service.GetMentorMentorTopicRequests(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<MentorTopicRequestResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MentorTopicRequestCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<MentorTopicRequestResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MentorTopicRequestUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<MentorTopicRequestResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] MentorTopicRequestRestoreCommand command)
        {
            var businessResult = await _service.Restore<MentorTopicRequestResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] MentorTopicRequestDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPost("student-request-idea")]
        public async Task<IActionResult> StudentRequest([FromBody] StudentRequest request)
        {
            var msg = await _service.StudentRequestIdea(request);
            return Ok(msg);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateMentorTopicRequestStatus([FromBody] MentorTopicRequestUpdateCommand request)
        {
            var businessResult = await _service.UpdateMentorTopicRequestStatus(request);
            return Ok(businessResult);
        }
    }
}
