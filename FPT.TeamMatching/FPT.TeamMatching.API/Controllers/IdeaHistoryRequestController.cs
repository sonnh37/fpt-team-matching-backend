using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistories;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistoryRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaHistories;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaHistoryRequests;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_IDEA_HISTORY_REQUESTS)]
    [ApiController]
    public class IdeaHistoryRequestController : ControllerBase
    {
        private readonly IIdeaHistoryRequestService _service;

        public IdeaHistoryRequestController(IIdeaHistoryRequestService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] IdeaHistoryRequestGetAllQuery query)
        {
            var msg = await _service.GetAll<IdeaHistoryRequestResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<IdeaHistoryRequestResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IdeaHistoryRequestCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<IdeaHistoryRequestResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] IdeaHistoryRequestUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<IdeaHistoryRequestResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] IdeaHistoryRequestRestoreCommand command)
        {
            var businessResult = await _service.Restore<IdeaHistoryRequestResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] IdeaHistoryRequestDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
