namespace BusinessLayer.Errors;

public enum ErrorType
{
    SnapshotNotFound,
    NoSnapshotsFound,

    DownloadError,

    EmailEmpty,
    NoSubscribersFound,
    InvalidEmailAddress,
    InvalidEmailCredentials,

    ConfigurationError,
}
