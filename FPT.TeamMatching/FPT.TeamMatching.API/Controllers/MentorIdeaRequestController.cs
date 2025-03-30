using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorIdeaRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.MentorIdeaRequest;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_MENTOR_IDEA_REQUESTS)]
    [ApiController]
    public class MentorIdeaRequestController : ControllerBase
    {
        private readonly IMentorIdeaRequestService _service;

        public MentorIdeaRequestController(IMentorIdeaRequestService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MentorIdeaRequestGetAllQuery query)
        {
            var msg = await _service.GetAll<MentorIdeaRequestResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<MentorIdeaRequestResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MentorIdeaRequestCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<MentorIdeaRequestResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MentorIdeaRequestUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<MentorIdeaRequestResult>(request);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] MentorIdeaRequestRestoreCommand command)
        {
            var businessResult = await _service.Restore<MentorIdeaRequestResult>(command);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] MentorIdeaRequestDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPost("student-request-idea")]
        public async Task<IActionResult> StudentRequest([FromBody] StudentRequest request)
        {
            var msg = await _service.StudentRequestIdea(request);
            return Ok(msg);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> MentorRespnse([FromBody] MentorIdeaRequestUpdateCommand request)
        {
            var businessResult = await _service.MentorResponse(request);
            return Ok(businessResult);
        }
    }
}
