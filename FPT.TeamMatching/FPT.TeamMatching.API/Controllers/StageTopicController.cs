using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.StageTopics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.StageTopics;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_STAGE_TOPICS)]
    [ApiController]
    [Authorize]
    public class StageTopicController : ControllerBase
    {
        private readonly IStageTopicService _service;

        public StageTopicController(IStageTopicService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] StageTopicGetAllQuery query)
        {
            var msg = await _service.GetAll<StageTopicResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<StageTopicResult>(id);
            return Ok(msg);
        }
        
        [HttpGet("stage-number/{number:int}")]
        public async Task<IActionResult> GetByStageNumber([FromRoute] int number)
        {
            var msg = await _service.GetByStageNumber<StageTopicResult>(number);
            return Ok(msg);
        }
        
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentStageTopic()
        {
            var msg = await _service.GetCurrentStageTopic<StageTopicResult>();
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StageTopicCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<StageTopicResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StageTopicUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<StageTopicResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] StageTopicRestoreCommand command)
        {
            var businessResult = await _service.Restore<StageTopicResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] StageTopicDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpGet("show-result/{stageTopicId:guid}")]
        public async Task<IActionResult> ShowResult([FromRoute] Guid stageTopicId)
        {
            var businessResult = await _service.ShowResult(stageTopicId);

            return Ok(businessResult);
        }
    }
}
