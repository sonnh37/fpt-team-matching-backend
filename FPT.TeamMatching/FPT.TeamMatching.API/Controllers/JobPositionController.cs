using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.JobPosition;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.JobPosition;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPT.TeamMatching.API.Controllers
{
    [Route("api/jobposition")]
    [ApiController]
    public class JobPositionController : ControllerBase
    {
        private readonly IJobPositionService _jobservice;

        public JobPositionController(IJobPositionService _service) {
        _jobservice = _service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] JobPositionGetAllQuery query)
        {
            var msg = await _jobservice.GetAll<JobPositionResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _jobservice.GetById<JobPositionResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JobPositionCreateCommand request)
        {
            var msg = await _jobservice.CreateOrUpdate<JobPositionResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] JobPositionUpdateCommand request)
        {
            var businessResult = await _jobservice.CreateOrUpdate<JobPositionResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] JobPositionRestoreCommand command)
        {
            var businessResult = await _jobservice.Restore<JobPositionResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] JobPositionDeleteCommand request)
        {
            var businessResult = await _jobservice.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
        }
    }

