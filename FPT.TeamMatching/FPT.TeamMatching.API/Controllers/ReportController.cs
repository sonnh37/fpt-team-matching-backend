using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reports;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Reports;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Tasks;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_REPORTS)]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;


        public ReportController(IReportService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ReportGetAllQuery query)
        {
            var msg = await _service.GetAll<ReportResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<ReportResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReportCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<ReportResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReportUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<ReportResult>(request);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] ReportDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
