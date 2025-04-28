using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CriteriaXCriteriaForms;
using FPT.TeamMatching.Domain.Models.Requests.Queries.CriteriaXCriteriaForms;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_CRITERIA_X_CRITERIA_FORMS)]
    [ApiController]
    [Authorize]
    public class CriteriaXCriteriaFormController : ControllerBase
    {
        private readonly ICriteriaXCriteriaFormService _service;

        public CriteriaXCriteriaFormController(ICriteriaXCriteriaFormService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CriteriaXCriteriaFormGetAllQuery query)
        {
            var msg = await _service.GetAll<CriteriaXCriteriaFormResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<CriteriaXCriteriaFormResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriteriaXCriteriaFormCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<CriteriaXCriteriaFormResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CriteriaXCriteriaFormUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<CriteriaXCriteriaFormResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] CriteriaXCriteriaFormRestoreCommand command)
        {
            var businessResult = await _service.Restore<CriteriaXCriteriaFormResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] CriteriaXCriteriaFormDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
