using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersions;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersions;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_IDEA_VERSIONS)]
    [ApiController]
    [Authorize]
    public class IdeaVersionController : ControllerBase
    {
        private readonly IIdeaVersionService _service;

        public IdeaVersionController(IIdeaVersionService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] IdeaVersionGetAllQuery query)
        {
            var msg = await _service.GetAll<IdeaVersionResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<IdeaVersionResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IdeaVersionCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<IdeaVersionResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] IdeaVersionUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<IdeaVersionResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] IdeaVersionRestoreCommand command)
        {
            var businessResult = await _service.Restore<IdeaVersionResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] IdeaVersionDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPost("resubmit-by-student-or-mentor")]
        public async Task<IActionResult> ResubmitByStudentOrMentor([FromBody] IdeaVersionResubmitByStudentOrMentor request)
        {
            var msg = await _service.ResubmitByStudentOrMentor(request);
            return Ok(msg);
        }
    }
}
