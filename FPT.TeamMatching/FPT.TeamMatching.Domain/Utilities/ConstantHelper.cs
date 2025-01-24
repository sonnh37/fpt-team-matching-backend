﻿using FPT.TeamMatching.Domain.Enums;

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

    public const string API_PROJECTS = $"{BaseApi}/projects";

    public const string API_PROJECT_ACTIVITIES = $"{BaseApi}/project-activities";

    public const string API_TASKS = $"{BaseApi}/tasks";

    public const string API_REPORTS = $"{BaseApi}/reports";

    public const string API_LECTURER_FEEDBACKS = $"{BaseApi}/lecturer-feedbacks";

    public const string API_INVITATION_USERS = $"{BaseApi}/invitation-users";

    public const string SortFieldDefault = "CreatedDate";

    #endregion
    
    #region Default get query

    public const int PageNumberDefault = 1;

    public const bool IsPagination = false;

    public const int PageSizeDefault = 10;

    public const SortOrder SortOrderDefault = SortOrder.Descending;

    #endregion
}