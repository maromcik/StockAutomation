using BusinessLayer.Errors;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer.Entities;

namespace BusinessLayer.Facades;

public class SendDifferencesFacade(IEmailService emailService, ISnapshotService snapshotService)
    : ISendDifferencesFacade
{
    public async Task<Result<bool, Error>> ProcessDiff(EmailSend snapshotCompare)
    {
        var diff =
            await snapshotService.CompareSnapshotsAsync(snapshotCompare.NewId, snapshotCompare.OldId);
        if (!diff.IsOk)
        {
            return diff.Error;
        }

        var (emailBody, emailAttachment) = await snapshotService.FormatDiff(diff.Value, OutputFormat.HTML);
        var email = await emailService.SendEmailAsync(emailBody, emailAttachment);
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

        var diff = await snapshotService.CompareLatestSnapshotsAsync();
        if (!diff.IsOk)
        {
            return diff.Error;
        }

        var (emailBody, emailAttachment) = await snapshotService.FormatDiff(diff.Value, OutputFormat.HTML);
        var email = await emailService.SendEmailAsync(emailBody, emailAttachment);
        if (!email.IsOk)
        {
            return email.Error;
        }

        return email.Value;
    }
}
