using BusinessLayer.Errors;
using BusinessLayer.Models;

namespace BusinessLayer.Facades;

public interface IProcessFacade
{
    public Task<Result<bool, Error>> ProcessDiff(EmailSend snapshotCompare);
}
