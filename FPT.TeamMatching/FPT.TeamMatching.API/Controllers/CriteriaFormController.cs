using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CriteriaForms;
using FPT.TeamMatching.Domain.Models.Requests.Queries.CriteriaForms;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_CRITERIA_FORMS)]
    [ApiController]
    [Authorize]
    public class CriteriaFormController : ControllerBase
    {
        private readonly ICriteriaFormService _service;

        public CriteriaFormController(ICriteriaFormService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CriteriaFormGetAllQuery query)
        {
            var msg = await _service.GetAll<CriteriaFormResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<CriteriaFormResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriteriaFormCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<CriteriaFormResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CriteriaFormUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<CriteriaFormResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] CriteriaFormRestoreCommand command)
        {
            var businessResult = await _service.Restore<CriteriaFormResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] CriteriaFormDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
