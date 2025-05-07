using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CapstoneSchedules;
using FPT.TeamMatching.Domain.Models.Requests.Queries.CapstoneSchedules;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_CAPSTONE_SCHEDULES)]
    [ApiController]
    [Authorize]
    public class CapstoneScheduleController : ControllerBase
    {
        private readonly ICapstoneScheduleService _service;

        public CapstoneScheduleController(ICapstoneScheduleService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CapstoneScheduleGetAllQuery query)
        {
            var msg = await _service.GetAll<CapstoneScheduleResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<CapstoneScheduleResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CapstoneScheduleCreateCommand request)
        {
            // var msg = await _service.CreateOrUpdate<CapstoneScheduleResult>(request);
            var msg = await _service.AddCapstoneSchedule(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CapstoneScheduleUpdateCommand request)
        {
            var businessResult = await _service.UpdateCapstoneSchedule(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] CapstoneScheduleRestoreCommand command)
        {
            var businessResult = await _service.Restore<CapstoneScheduleResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] CapstoneScheduleDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcelFile([FromForm]CapstoneImportRequest request)
        {
            var businessResult = await _service.ImportExcelFile(request.file, request.Stage);
            return Ok(businessResult);
        }

        [HttpPost("get-by-semester-id-and-stage")]
        public async Task<IActionResult> GetBySemesterIdAndStage([FromBody] CapstoneScheduleFilter command)
        {
            var msg = await _service.GetBySemesterIdAndStage(command);
            return Ok(msg);
        }

        [HttpGet("get-by-project-id/{projectId:guid}")]
        public async Task<IActionResult> GetByProjectId(Guid projectId)
        {
            var msg = await _service.GetByProjectId(projectId);
            return Ok(msg);
        }

        [HttpPut("update-demo")]
        public async Task<IActionResult> UpdateDemo([FromQuery] Guid capstoneScheduleId)
        {
            var msg = await _service.UpdateCapstoneScheduleDemo(capstoneScheduleId);
            return Ok(msg);
        }
    }
}
