using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Utilities;

public static class Const
{
    #region Error Codes

    public const int ERROR_EXCEPTION_CODE = -4;

    #endregion

    #region Success Codes

    public const int SUCCESS_CODE = 1;

    public const string SUCCESS_SAVE_MSG = "Data has been saved successfully.";
    public const string SUCCESS_READ_MSG = "Data retrieved successfully.";
    public const string SUCCESS_DELETE_MSG = "Data deleted successfully.";

    #endregion

    #region Fail code

    public const int FAIL_CODE = -1;
    public const string FAIL_SAVE_MSG = "Save fail";
    public const string FAIL_READ_MSG = "Get fail";
    public const string FAIL_DELETE_MSG = "Delete fail";

    #endregion

    #region Not Found Codes

    public const int NOT_FOUND_CODE = -2;
    public const string NOT_FOUND_MSG = "Not found";

    #endregion

    #region Url api

    private const string BaseApi = "api";

    public const string API_USERS = $"{BaseApi}/users";

    public const string API_REFRESH_TOKENS = $"{BaseApi}/refresh-tokens";

    public const string API_PROJECTS = $"{BaseApi}/projects";

    public const string API_PROJECT_ACTIVITIES = $"{BaseApi}/project-activities";

    public const string API_TASKS = $"{BaseApi}/tasks";

    public const string API_REVIEWS = $"{BaseApi}/reviews";

    public const string API_LECTURER_FEEDBACKS = $"{BaseApi}/lecturer-feedbacks";

    public const string API_INVITATION_USERS = $"{BaseApi}/invitation-users";
    public const string API_BLOGS = $"{BaseApi}/blogs";
    public const string API_COMMENTS = $"{BaseApi}/comments";
    public const string API_LIKES = $"{BaseApi}/likes";
    public const string API_APPLICATIONS = $"{BaseApi}/applications";
    public const string API_RATES = $"{BaseApi}/rates";
    public const string API_TEAM_MEMBERS = $"{BaseApi}/team-members";

    public const string API_NOTIFICATIONS = $"{BaseApi}/notifications";

    public const string API_PROFILES = $"{BaseApi}/profiles";

    public const string API_SKILLPROFILES = $"{BaseApi}/skill-profiles";

    public const string? API_VERIFY_QUALIFIED = $"{BaseApi}/verify-qualified";

    public const string API_VERIFY_SEMESTER = $"{BaseApi}/verify-semester";

    public const string API_MESSAGE = $"{BaseApi}/message";
    public const string API_CONVERSATION_MEMBER = $"{BaseApi}/conversation-members";

    public const string SortFieldDefault = "CreatedDate";

    public const string HANGFIRE = $"{BaseApi}/hangfire";

    #endregion

    #region Default get query

    public const int PageNumberDefault = 1;

    public const bool IsPagination = false;

    public const int PageSizeDefault = 10;

    public const SortOrder SortOrderDefault = SortOrder.Descending;

    #endregion
}