using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_TOPICS)]
[ApiController]
[Authorize]
public class TopicController : ControllerBase
{
    private readonly ITopicService _service;

    public TopicController(ITopicService __service)
    {
        _service = __service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TopicGetAllQuery query)
    {
        var msg = await _service.GetAll<TopicResult>(query);
        return Ok(msg);
    }
    
    [HttpGet("supervisors")]
    public async Task<IActionResult> GetIdeasOfSupervisors([FromQuery] TopicGetListOfSupervisorsQuery query)
    {
        var msg = await _service.GetTopicsOfSupervisors<TopicResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _service.GetById<TopicResult>(id);
        return Ok(msg);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TopicCreateCommand request)
    {
        var msg = await _service.CreateOrUpdate<TopicResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TopicUpdateCommand request)
    {
        var businessResult = await _service.UpdateTopic(request);

        return Ok(businessResult);
    }

    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] TopicUpdateStatusCommand request)
    {
        var businessResult = await _service.UpdateStatusTopic(request);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] TopicDeleteCommand request)
    {
        var businessResult = await _service.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] TopicRestoreCommand command)
    {
        var businessResult = await _service.Restore<TopicResult>(command);

        return Ok(businessResult);
    }

    [HttpPost("submit-to-mentor-by-student")]
    public async Task<IActionResult> SubmitToMentorByStudent([FromBody] TopicSubmitForMentorByStudentCommand request)
    {
        var msg = await _service.SubmitToMentorByStudent(request);
        return Ok(msg);
    }

    [HttpPost("resubmit-to-mentor-by-student")]
    public async Task<IActionResult> ResubmitToMentorByStudent([FromBody] TopicResubmitForMentorByStudentCommand request)
    {
        var msg = await _service.ResubmitToMentorByStudent(request);
        return Ok(msg);
    }

    [HttpPost("submit-topic-of-lecturer-by-lecturer")]
    public async Task<IActionResult> SubmitTopicOfLecturerByLecturer([FromBody] TopicLecturerCreatePendingCommand request)
    {
        var msg = await _service.SubmitTopicOfLecturerByLecturer(request);
        return Ok(msg);
    }

    [HttpPut("submit-topic-of-student-by-lecturer/{topicId:guid}")]
    public async Task<IActionResult> SubmitTopicOfStudentByLecturer([FromRoute] Guid topicId)
    {
        var msg = await _service.SubmitTopicOfStudentByLecturer(topicId);
        return Ok(msg);
    }
    
    [HttpPut("select-as-project")]
    public async Task<IActionResult> SubmitTopicOfStudentByLecturer([FromBody] TopicUpdateAsProjectCommand request)
    {
        var msg = await _service.UpdateTopicAsProject(request);
        return Ok(msg);
    }

    [HttpGet("me/by-status-and-roles")]
    public async Task<IActionResult> GetIdeaRequestsCurrentByStatusAndRoles([FromQuery] TopicRequestGetListByStatusAndRoleQuery query)
    {
        var msg = await _service.GetTopicsOfReviewerByRolesAndStatus<TopicResult>(query);
        return Ok(msg);
    }

    [HttpGet("get-by-user-id")]
    public async Task<IActionResult> GetByUserIdLogin()
    {
        var businessResult = await _service.GetTopicsByUserId();
        return Ok(businessResult);
    }
    
    [HttpGet("me/by-list-status")]
    public async Task<IActionResult> GetUserIdeasByStatus([FromQuery] TopicGetListForUserByStatus request)
    {
        var businessResult = await _service.GetUserTopicsByStatus(request);
        return Ok(businessResult);
    }

    [HttpGet("get-approved-topics-do-not-have-team")]
    public async Task<IActionResult> GetApprovedTopicsDoNotHaveTeam()
    {
        var businessResult = await _service.GetApprovedTopicsDoNotHaveTeam();
        return Ok(businessResult);
    }
    
    [HttpGet("me/mentor-topics")]
    public async Task<IActionResult> GetTopicsForMentor([FromQuery] TopicGetListForMentorQuery query)
    {
        var businessResult = await _service.GetTopicsForMentor(query);
        return Ok(businessResult);
    }

    [HttpPost("create-draft")]
    public async Task<IActionResult> CreateDraft([FromBody] TopicCreateDraftCommand command)
    {
        var businessResult = await _service.CreateDraft(command);
        return Ok(businessResult);
    }

    [HttpPost("update-draft")]
    public async Task<IActionResult> UpdateDraft([FromQuery] TopicUpdateDraftCommand command)
    {
        var businessResult = await _service.UpdateDraft(command);
        return Ok(businessResult);
    }

}

