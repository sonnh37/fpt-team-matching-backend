using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.UserXRoles;
using FPT.TeamMatching.Domain.Models.Requests.Queries.UserXRoles;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers
{
    [Route(Const.API_USER_X_ROLES)]
    [ApiController]
    [Authorize]
    public class UserXRoleController : ControllerBase
    {
        private readonly IUserXRoleService _service;

        public UserXRoleController(IUserXRoleService __service)
        {
            _service = __service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserXRoleGetAllQuery query)
        {
            var msg = await _service.GetAll<UserXRoleResult>(query);
            return Ok(msg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var msg = await _service.GetById<UserXRoleResult>(id);
            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserXRoleCreateCommand request)
        {
            var msg = await _service.CreateOrUpdate<UserXRoleResult>(request);
            return Ok(msg);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserXRoleUpdateCommand request)
        {
            var businessResult = await _service.CreateOrUpdate<UserXRoleResult>(request);
            return Ok(businessResult);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore([FromBody] UserXRoleRestoreCommand command)
        {
            var businessResult = await _service.Restore<UserXRoleResult>(command);
            return Ok(businessResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] UserXRoleDeleteCommand request)
        {
            var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

            return Ok(businessResult);
        }
    }
}
