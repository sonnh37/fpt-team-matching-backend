using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Professions;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Professions;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_PROFESSIONS)]
    [ApiController]
    [Authorize]
    public class ProfessionController : ControllerBase
    {
        private readonly IProfessionService _service;


        public ProfessionController(IProfessionService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProfessionGetAllQuery query)
        {
            var msg = await _service.GetAll<ProfessionResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<ProfessionResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProfessionCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<ProfessionResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProfessionUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<ProfessionResult>(request);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] ProfessionDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] ProfessionRestoreCommand command)
        {
            var businessResult = await _service.Restore<ProfessionResult>(command);

            return Ok(businessResult);
        }
    }
}
