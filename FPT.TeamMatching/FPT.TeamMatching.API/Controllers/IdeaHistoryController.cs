using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistories;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaHistories;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_IDEA_HISTORIES)]
    [ApiController]
    public class IdeaHistoryController : ControllerBase
    {
        private readonly IIdeaHistoryService _service;

        public IdeaHistoryController(IIdeaHistoryService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] IdeaHistoryGetAllQuery query)
        {
            var msg = await _service.GetAll<IdeaHistoryResult>(query);
            return Ok(msg);
        }
        [HttpGet("idea/{ideaId:guid}")]
        public async Task<IActionResult> GetAll([FromRoute] Guid ideaId)
        {
            var msg = await _service.GetAllIdeaHistoryByIdeaId(ideaId);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<IdeaHistoryResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IdeaHistoryCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<IdeaHistoryResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] IdeaHistoryUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<IdeaHistoryResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] IdeaHistoryRestoreCommand command)
        {
            var businessResult = await _service.Restore<IdeaHistoryResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] IdeaHistoryDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPost("student-update-idea")]
        public async Task<IActionResult> StudentUpdateIdea([FromBody] StudentUpdateIdeaCommand request)
        {
            var msg = await _service.StudentUpdateIdea(request);
            return Ok(msg);
        }

        [HttpPut("lecturer-update")]
        public async Task<IActionResult> LecturerUpdate([FromBody] LecturerUpdateCommand command)
        {
            var businessResult = await _service.LecturerUpdate(command);

            return Ok(businessResult);
        }
    }
}
