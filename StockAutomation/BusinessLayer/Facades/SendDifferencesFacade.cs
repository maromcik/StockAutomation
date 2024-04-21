using BusinessLayer.Errors;
using BusinessLayer.Models;
using BusinessLayer.Services;

namespace BusinessLayer.Facades;

public class SendDifferencesFacade(IEmailService emailService, ISnapshotService snapshotService) : ISendDifferencesFacade
{
    public async Task<Result<bool, Error>> ProcessDiff(EmailSend emailSend)
    {
        var result = await snapshotService.CompareSnapshotsAsync(emailSend.NewId, emailSend.OldId);
        if (!result.IsOk)
        {
            return result.Error;
        }

        // pass result.Value here
        var email = await emailService.SendEmailAsync();
        if (!email.IsOk)
        {
            return email.Error;
        }

        return email.Value;
    }
}
