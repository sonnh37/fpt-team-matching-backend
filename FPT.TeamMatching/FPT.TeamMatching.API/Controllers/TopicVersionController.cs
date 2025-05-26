using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersions;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicVersions;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_TOPIC_VERSIONS)]
    [ApiController]
    [Authorize]
    public class TopicVersionController : ControllerBase
    {
        private readonly ITopicVersionService _service;

        public TopicVersionController(ITopicVersionService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TopicVersionGetAllQuery query)
        {
            var msg = await _service.GetAll<TopicVersionResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<TopicVersionResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TopicVersionCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<TopicVersionResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TopicVersionUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<TopicVersionResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] TopicVersionRestoreCommand command)
        {
            var businessResult = await _service.Restore<TopicVersionResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] TopicVersionDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPost("update-topic-by-student")]
        public async Task<IActionResult> UpdateTopicByStudent([FromBody] UpdateTopicByStudentCommand command)
        {
            var msg = await _service.UpdateByStudent(command);
            return Ok(msg);
        }

        [HttpGet("get-by-topic-version-id")]
        public async Task<IActionResult> GetByTopicId([FromQuery] Guid id)
        {
            var msg = await _service.GetAllTopicVersionByIdeaId(id);
            return Ok(msg);
        }
    }
}
