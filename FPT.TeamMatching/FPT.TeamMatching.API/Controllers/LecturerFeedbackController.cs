using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.LecturerFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reports;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.LecturerFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Reports;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_LECTURER_FEEDBACKS)]
    [ApiController]
    public class LecturerFeedbackController : ControllerBase
    {
        private readonly ILecturerFeedbackService _service;


        public LecturerFeedbackController(ILecturerFeedbackService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] LecturerFeedbackGetAllQuery query)
        {
            var msg = await _service.GetAll<LecturerFeedbackResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<LecturerFeedbackResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LecturerFeedbackCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<LecturerFeedbackResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LecturerFeedbackUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<LecturerFeedbackResult>(request);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] LecturerFeedbackDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] LecturerFeedbackRestoreCommand command)
        {
            var businessResult = await _service.Restore<LecturerFeedbackResult>(command);

            return Ok(businessResult);
        }
    }
}
