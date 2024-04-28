namespace BusinessLayer.Errors;

public enum ErrorType
{
    SnapshotEmpty,
    SnapshotNotFound,
    NoSnapshotsFound,

    DownloadError,

    EmailEmpty,
    NoSubscribersFound,
    InvalidEmailAddress,
    InvalidEmailCredentials,

    ConfigurationError,
}
