using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Timelines;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Timelines;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_TIMELINES)]
    [ApiController]
    [Authorize]
    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _service;

        public TimelineController(ITimelineService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TimelineGetAllQuery query)
        {
            var msg = await _service.GetAll<TimelineResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<TimelineResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimelineCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<TimelineResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TimelineUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<TimelineResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] TimelineRestoreCommand command)
        {
            var businessResult = await _service.Restore<TimelineResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] TimelineDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
