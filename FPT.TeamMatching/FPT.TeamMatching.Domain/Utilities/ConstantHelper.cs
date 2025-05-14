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

    public const string SUCCESS_SAVE_MSG = "Lưu dữ liệu thành công.";
    public const string SUCCESS_DELETE_MSG = "Xoá dữ liệu thành công.";
    public const string SUCCESS_READ_MSG = "Đọc dữ liệu thành công.";

    #endregion

    #region Fail code

    public const int FAIL_CODE = -1;
    public const string FAIL_SAVE_MSG = "Lưu dữ liệu thất bại.";
    public const string FAIL_READ_MSG = "Lỗi khi đọc dữ liệu.";
    public const string FAIL_DELETE_MSG = "Xoá dữ liệu thất bại.";

    #endregion

    #region Not Found Codes

    public const int NOT_FOUND_CODE = -2;
    public const string NOT_FOUND_MSG = "Không tìm thấy dữ liệu.";

    #endregion

    #region Authorization Codes

    public const int FAIL_UNAUTHORIZED_CODE = -5;
    public const string FAIL_UNAUTHORIZED_MSG = "Truy cập không được phép.";
    public const string FAIL_FORBIDDEN_MSG = "Từ chối truy cập. Bạn không có quyền thực hiện thao tác này.";
    #endregion
    
    #region Url api

    private const string BaseApi = "api";

    public const string API_USERS = $"{BaseApi}/users";

    public const string API_ROLES = $"{BaseApi}/roles";

    public const string API_USER_X_ROLES = $"{BaseApi}/user-x-roles";

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

    public const string API_TOPICS = $"{BaseApi}/topics";

    //public const string API_TOPIC_REGISTERS = $"{BaseApi}/topic-registers";

    public const string API_TOPIC_REQUESTS = $"{BaseApi}/topic-requests";

    public const string API_TOPIC_VERSIONS = $"{BaseApi}/topic-versions";

    public const string API_TOPIC_VERSION_REQUESTS = $"{BaseApi}/topic-version-requests";

    public const string API_TEAM_MEMBERS = $"{BaseApi}/team-members";

    public const string API_NOTIFICATIONS = $"{BaseApi}/notifications";

    public const string API_PROFILE_STUDENTS = $"{BaseApi}/profile-students";

    public const string API_SKILL_PROFILES = $"{BaseApi}/skill-profiles";

    public const string API_MESSAGE = $"{BaseApi}/message";

    public const string API_CONVERSATION_MEMBER = $"{BaseApi}/conversation-members";

    public const string API_PROFESSIONS = $"{BaseApi}/professions";

    public const string API_SPECIALTIES = $"{BaseApi}/specialties";

    public const string API_SEMESTERS = $"{BaseApi}/semesters";

    public const string API_STAGE_TOPICS = $"{BaseApi}/stage-topics";

    public const string API_CAPSTONE_SCHEDULES = $"{BaseApi}/capstone-schedules";

    public const string API_MENTOR_TOPIC_REQUESTS = $"{BaseApi}/mentor-topic-requests";

    public const string API_MENTOR_FEEDBACKS = $"{BaseApi}/mentor-feedbacks";

    //public const string API_TIMELINES = $"{BaseApi}/timelines";

    public const string API_CRITERIAS = $"{BaseApi}/criterias";

    public const string API_CRITERIA_FORMS = $"{BaseApi}/criteria-forms";

    public const string API_CRITERIA_X_CRITERIA_FORMS = $"{BaseApi}/criteria-x-criteria-forms";

    public const string API_ANSWER_CRITERIAS = $"{BaseApi}/answer-criterias";

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