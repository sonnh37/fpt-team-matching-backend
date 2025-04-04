using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Utilities;

public static class Const
{
    #region Error Codes

    public const int ERROR_SYSTEM_CODE = -4;
    public const string ERROR_SYSTEM_MSG = "System error occurred. Please try again or contact support.";
    #endregion

    #region Success Codes

    public const int SUCCESS_CODE = 1;

    public const string SUCCESS_SAVE_MSG = "Saved successfully.";
    public const string SUCCESS_DELETE_MSG = "Deleted successfully.";
    public const string SUCCESS_READ_MSG = "Read successfully.";

    #endregion

    #region Fail code

    public const int FAIL_CODE = -1;
    public const string FAIL_SAVE_MSG = "Failed to save data.";
    public const string FAIL_READ_MSG = "Failed to read data.";
    public const string FAIL_DELETE_MSG = "Failed to delete data.";

    #endregion

    #region Not Found Codes

    public const int NOT_FOUND_CODE = -2;
    public const string NOT_FOUND_MSG = "The requested resource was not found.";

    #endregion

    #region Authorization Codes

    public const int FAIL_UNAUTHORIZED_CODE = -5;
    public const string FAIL_UNAUTHORIZED_MSG = "Unauthorized access.";
    public const string FAIL_FORBIDDEN_MSG = "Access denied. You don't have permission.";

    #endregion

    
    #region Url api

    private const string BaseApi = "api";

    public const string API_USERS = $"{BaseApi}/users";

    public const string API_REFRESH_TOKENS = $"{BaseApi}/refresh-tokens";

    public const string API_PROJECTS = $"{BaseApi}/projects";

    public const string API_REVIEWS = $"{BaseApi}/reviews";

    public const string API_EXPIRATION_REVIEWS = $"{BaseApi}/expiration-reviews";

    public const string API_FEEDBACKS = $"{BaseApi}/feedbacks";

    public const string API_INVITATIONS = $"{BaseApi}/invitations";

    public const string API_BLOGS = $"{BaseApi}/blogs";

    public const string API_COMMENTS = $"{BaseApi}/comments";

    public const string API_LIKES = $"{BaseApi}/likes";

    public const string API_AUTH = $"{BaseApi}/auth";

    public const string API_BLOGCVS = $"{BaseApi}/blog-cvs";

    public const string API_RATES = $"{BaseApi}/rates";

    public const string API_IDEAS = $"{BaseApi}/ideas";

    public const string API_IDEA_REQUESTS = $"{BaseApi}/idea-requests";

    public const string API_TEAM_MEMBERS = $"{BaseApi}/team-members";

    public const string API_NOTIFICATIONS = $"{BaseApi}/notifications";

    public const string API_PROFILE_STUDENTS = $"{BaseApi}/profile-students";

    public const string API_SKILLPROFILES = $"{BaseApi}/skill-profiles";

    public const string API_MESSAGE = $"{BaseApi}/message";

    public const string API_CONVERSATION_MEMBER = $"{BaseApi}/conversation-members";

    public const string API_PROFESSIONS = $"{BaseApi}/professions";

    public const string API_SPECIALTIES = $"{BaseApi}/specialties";

    public const string API_SEMESTERS = $"{BaseApi}/semesters";

    public const string API_STAGE_IDEAS = $"{BaseApi}/stage-ideas";

    public const string API_CAPSTONE_SCHEDULES = $"{BaseApi}/capstone-schedules";

    public const string API_MENTOR_IDEA_REQUESTS = $"{BaseApi}/mentor-idea-requests";

    public const string API_IDEA_HISTORIES = $"{BaseApi}/idea-histories";

    public const string API_IDEA_HISTORY_REQUESTS = $"{BaseApi}/idea-history-requests";

    public const string API_MENTOR_FEEDBACKS = $"{BaseApi}/mentor-feedbacks";

    public const string SortFieldDefault = "CreatedDate";

    public const string HANGFIRE = $"{BaseApi}/hangfire";

    public const string API_HUBS = $"{BaseApi}/api_hubs";
    
    public const string API_FILE_UPLOAD = $"{BaseApi}/file-upload";

    #endregion

    #region Default get query

    public const int PageNumberDefault = 1;

    public const bool IsPagination = false;

    public const bool IsPermanent = false;

    public const int PageSizeDefault = 10;

    public const SortOrder SortOrderDefault = SortOrder.Descending;

    #endregion
}