namespace BusinessLayer.Errors;

public enum ErrorType
{
    FileNotFound,

    SnapshotNotFound,
    NoSnapshotsFound,

    DownloadError,

    EmailEmpty,
    NoSubscribersFound,
    InvalidEmailAddress,
    InvalidEmailCredentials,

    ConfigurationError,
}
