namespace BusinessLayer.Errors;

public enum ErrorType
{
    SnapshotEmpty,
    SnapshotNotFound,
    SnapshotsNotFound,

    DownloadError,

    EmailEmpty,
    SubscribersNotFound,
    InvalidEmailAddress,
    InvalidEmailCredentials,

    ConfigurationError,
}
