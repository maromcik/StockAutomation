using BusinessLayer.Errors;
using BusinessLayer.Models;

namespace BusinessLayer.Facades;

public interface IProcessDiffFacade
{
    public Task<Result<bool, Error>> ProcessSendDiff(EmailSend snapshotCompare);
    public Task<Result<bool, Error>> ProcessSendDiffLatest();
    public Task<Result<string, Error>> ProcessDiffLatest();
}