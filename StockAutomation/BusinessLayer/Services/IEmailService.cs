using BusinessLayer.Errors;
using BusinessLayer.Models;

namespace BusinessLayer.Services;

public interface IEmailService
{
    public Task<Result<bool, Error>> SendEmailAsync(string emailBody, string? attachmentContent);
    public Task<Result<bool, Error>> SaveEmailSettingsAsync(FormatSettings settings);

    public Task<FormatSettings> GetEmailSettings();
}