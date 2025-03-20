using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Semester;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Semester;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_SEMESTERS)]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _service;

        public SemesterController(ISemesterService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SemesterGetAllQuery query)
        {
            var msg = await _service.GetAll<SemesterResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<SemesterResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SemesterCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<SemesterResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SemesterUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<SemesterResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] SemesterRestoreCommand command)
        {
            var businessResult = await _service.Restore<SemesterResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] SemesterDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpGet("get-present-semester")]
        public async Task<IActionResult> GetPresentSemester()
        {
            var msg = await _service.GetPresentSemester();
            return Ok(msg);
        }
    }
}
