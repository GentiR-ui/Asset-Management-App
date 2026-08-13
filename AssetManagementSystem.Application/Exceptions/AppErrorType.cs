namespace AssetManagementSystem.Application.Exceptions;


public enum AppErrorType
{
    /// <summary>Input failed validation rules. Usually maps to 400.</summary>
    Validation,

    /// <summary>The requested resource does not exist. Usually maps to 404.</summary>
    NotFound,

    /// <summary>The request conflicts with current state, e.g. a duplicate email. Usually maps to 409.</summary>
    Conflict,

    /// <summary>The caller is not authenticated — we do not know who they are. Usually maps to 401.</summary>
    Unauthorized,

    /// <summary>The caller is authenticated but not allowed to do this. Usually maps to 403.</summary>
    Forbidden,

    /// <summary>The request is malformed or nonsensical for a reason other than validation. Usually maps to 400.</summary>
    BadRequest,

    /// <summary>Something genuinely went wrong that we did not anticipate. Usually maps to 500.</summary>
    Unexpected
}
