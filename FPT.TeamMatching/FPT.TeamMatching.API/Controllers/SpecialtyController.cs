using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Specialties;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Specialties;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_SPECIALTIES)]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISpecialtyService _service;


        public SpecialtyController(ISpecialtyService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SpecialtyGetAllQuery query)
        {
            var msg = await _service.GetAll<SpecialtyResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<SpecialtyResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecialtyCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<SpecialtyResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SpecialtyUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<SpecialtyResult>(request);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] SpecialtyDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] SpecialtyRestoreCommand command)
        {
            var businessResult = await _service.Restore<SpecialtyResult>(command);

            return Ok(businessResult);
        }
    }
}
