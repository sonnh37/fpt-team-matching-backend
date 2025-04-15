using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Criterias;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Criterias;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_CRITERIAS)]
    [ApiController]
    public class CriteriaController : ControllerBase
    {
        private readonly ICriteriaService _service;

        public CriteriaController(ICriteriaService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CriteriaGetAllQuery query)
        {
            var msg = await _service.GetAll<CriteriaResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<CriteriaResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriteriaCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<CriteriaResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CriteriaUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<CriteriaResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] CriteriaRestoreCommand command)
        {
            var businessResult = await _service.Restore<CriteriaResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] CriteriaDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
