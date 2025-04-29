using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.AnswerCriterias;
using FPT.TeamMatching.Domain.Models.Requests.Queries.AnswerCriterias;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_ANSWER_CRITERIAS)]
    [ApiController]
    [Authorize]
    public class AnswerCriteriaController : ControllerBase
    {
        private readonly IAnswerCriteriaService _service;

        public AnswerCriteriaController(IAnswerCriteriaService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AnswerCriteriaGetAllQuery query)
        {
            var msg = await _service.GetAll<AnswerCriteriaResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<AnswerCriteriaResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AnswerCriteriaCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<AnswerCriteriaResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AnswerCriteriaUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<AnswerCriteriaResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] AnswerCriteriaRestoreCommand command)
        {
            var businessResult = await _service.Restore<AnswerCriteriaResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] AnswerCriteriaDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
