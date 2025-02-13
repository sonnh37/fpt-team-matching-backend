using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProjectActivities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reports;
using FPT.TeamMatching.Domain.Models.Requests.Queries.ProjectActivities;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Reports;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_PROJECT_ACTIVITIES)]
    [ApiController]
    public class ProjectActivityController : ControllerBase
    {
        private readonly IProjectActivityService _service;


        public ProjectActivityController(IProjectActivityService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProjectActivityGetAllQuery query)
        {
            var msg = await _service.GetAll<ProjectActivityResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<ProjectActivityResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectActivityCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<ProjectActivityResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProjectActivityUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<ProjectActivityResult>(request);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] ProjectActivityDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] ProjectActivityRestoreCommand command)
        {
            var businessResult = await _service.Restore<ProjectActivityResult>(command);

            return Ok(businessResult);
        }
    }
}
}
