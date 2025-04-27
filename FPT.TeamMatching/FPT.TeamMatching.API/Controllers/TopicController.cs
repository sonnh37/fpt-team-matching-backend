using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_TOPICS)]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _service;

        public TopicController(ITopicService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TopicGetAllQuery query)
        {
            var msg = await _service.GetAll<TopicResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<TopicResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TopicCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<TopicResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TopicUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<TopicResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] TopicRestoreCommand command)
        {
            var businessResult = await _service.Restore<TopicResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] TopicDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
        
        [HttpGet("me/mentor-topics")]
        public async Task<IActionResult> GetTopicsForMentor([FromQuery] TopicGetListForMentorQuery query)
        {
            var businessResult = await _service.GetTopicsForMentor(query);
            return Ok(businessResult);
        }

    }
}
