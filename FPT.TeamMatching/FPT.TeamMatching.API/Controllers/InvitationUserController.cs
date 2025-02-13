using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.InvitationUsers;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reports;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Queries.InvitationUsers;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Reports;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_INVITATION_USERS)]
    [ApiController]
    public class InvitationUserController : ControllerBase
    {
        private readonly IInvitationUserService _service;


        public InvitationUserController(IInvitationUserService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] InvitationUserGetAllQuery query)
        {
            var msg = await _service.GetAll<InvitationUserResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<InvitationUserResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvitationUserCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<InvitationUserResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] InvitationUserUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<InvitationUserResult>(request);

            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] InvitationUserDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] InvitationUserRestoreCommand command)
        {
            var businessResult = await _service.Restore<InvitationUserResult>(command);

            return Ok(businessResult);
        }
    }
}
