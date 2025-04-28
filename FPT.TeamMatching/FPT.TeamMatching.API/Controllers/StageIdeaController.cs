using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.StageIdeas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.StageIdeas;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_STAGE_IDEAS)]
    [ApiController]
    [Authorize]
    public class StageIdeaController : ControllerBase
    {
        private readonly IStageIdeaService _service;

        public StageIdeaController(IStageIdeaService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] StageIdeaGetAllQuery query)
        {
            var msg = await _service.GetAll<StageIdeaResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<StageIdeaResult>(id);
            return Ok(msg);
        }
        
        [HttpGet("stage-number/{number:int}")]
        public async Task<IActionResult> GetByStageNumber([FromRoute] int number)
        {
            var msg = await _service.GetByStageNumber<StageIdeaResult>(number);
            return Ok(msg);
        }
        
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentStageIdea()
        {
            var msg = await _service.GetCurrentStageIdea<StageIdeaResult>();
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StageIdeaCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<StageIdeaResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StageIdeaUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<StageIdeaResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] StageIdeaRestoreCommand command)
        {
            var businessResult = await _service.Restore<StageIdeaResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] StageIdeaDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
