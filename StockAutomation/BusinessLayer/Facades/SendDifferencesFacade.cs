using BusinessLayer.Errors;
using BusinessLayer.Models;
using BusinessLayer.Services;

namespace BusinessLayer.Facades;

public class SendDifferencesFacade(IEmailService emailService, ISnapshotService snapshotService)
    : ISendDifferencesFacade
{
    public async Task<Result<bool, Error>> ProcessDiff(EmailSend snapshotCompare)
    {
        var result = await snapshotService.CompareSnapshotsAsync(snapshotCompare.NewId, snapshotCompare.OldId);
        if (!result.IsOk)
        {
            return result.Error;
        }

        var (emailBody, attachmentContent) = result.Value;
        var email = await emailService.SendEmailAsync(emailBody, attachmentContent);
        if (!email.IsOk)
        {
            return email.Error;
        }

        return email.Value;
    }

    public async Task<Result<bool, Error>> ProcessDiffLatest()
    {
        var download = await snapshotService.DownloadSnapshotAsync();
        if (!download.IsOk)
        {
            return download.Error;
        }

        var result = await snapshotService.CompareLatestSnapshotsAsync();
        if (!result.IsOk)
        {
            return result.Error;
        }

        var (emailBody, emailAttachment)= result.Value;
        var email = await emailService.SendEmailAsync(emailBody, emailAttachment);
        if (!email.IsOk)
        {
            return email.Error;
        }

        return email.Value;
    }
}