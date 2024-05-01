using BusinessLayer.Errors;
using BusinessLayer.Models;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Services;

using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly StockAutomationDbContext _context;
    private readonly IConfiguration _configuration;

    public EmailService(StockAutomationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        var config = _context.Configurations.FirstOrDefault();
        if (config == null)
        {
            _context.Configurations.Add(new Configuration
            {
                Id = 0,
                OutputFormat = OutputFormat.Text
            });
            _context.SaveChanges();
        }
    }

    public async Task<IEnumerable<Subscriber>> GetSubscribersAsync()
    {
        return await _context.Subscribers.ToListAsync();
    }

    public async Task<Result<bool, Error>> SendEmailAsync(string diff)
    {
        var subscribers = await _context.Subscribers.ToListAsync();
        var smtpConfig = _configuration.GetSection("SMTP");

        var host = smtpConfig["Host"];
        var port = Convert.ToInt32(smtpConfig["Port"]);
        var username = smtpConfig["Username"];
        var password = smtpConfig["Password"];
        var from = smtpConfig["From"];

        if (string.IsNullOrEmpty(host) ||
            port == 0 ||
            string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(from))
        {
            return new Error
            {
                ErrorType = ErrorType.InvalidEmailCredentials,
                Message = "Invalid credentials"
            };
        }

        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(from),
            Subject = "Update in holdings is here!",
            Body = CreateEmailBody(diff),
            IsBodyHtml = true,
        };

        foreach (var subscription in subscribers)
        {
            mailMessage.Bcc.Add(subscription.EmailAddress);
        }

        smtpClient.Send(mailMessage);
        return true;
    }

    public async Task<Result<bool, Error>> SaveEmailSettingsAsync(FormatSettings settings)
    {
        var dbConfig = await _context.Configurations.FirstOrDefaultAsync();
        if (dbConfig == null)
        {
            return new Error
            {
                ErrorType = ErrorType.ConfigurationError,
                Message = "No default config found."
            };
        }

        dbConfig.OutputFormat = settings.PreferredFormat;
        _context.Configurations.Update(dbConfig);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<FormatSettings> GetEmailSettings()
    {
        var config = await _context.Configurations.FirstOrDefaultAsync();
        var settings = new FormatSettings(config?.OutputFormat ?? OutputFormat.Text);
        return settings;
    }

    public async Task<Result<bool, Error>> CreateSubscriber(SubscriberCreate subscriberCreate)
    {
        if (string.IsNullOrEmpty(subscriberCreate.EmailAddress))
        {
            return new Error
            {
                ErrorType = ErrorType.EmailEmpty,
                Message = "Email address cannot be empty"
            };
        }

        if (!IsEmailAddressValid(subscriberCreate.EmailAddress))
        {
            return new Error
            {
                ErrorType = ErrorType.InvalidEmailAddress,
                Message = "Invalid email address"
            };
        }

        var subscriber = new Subscriber
        {
            EmailAddress = subscriberCreate.EmailAddress
        };
        _context.Add(subscriber);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool, Error>> DeleteSubscribersAsync(List<int> ids)
    {
        var subscribers = await _context.Subscribers.Where(s => ids.Contains(s.Id)).ToListAsync();
        if (subscribers.Count == 0)
        {
            return new Error
            {
                ErrorType = ErrorType.NoSubscribersFound,
                Message = "Could not delete selected subscribers - not found"
            };
        }

        _context.Subscribers.RemoveRange(subscribers);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SubscriberView> SearchSubscribersAsync(PaginationSettings? paginationSettings, string? query)
    {
        var subscribers = _context.Subscribers.AsQueryable();

        if (paginationSettings != null)
        {
            var subscribersCount = await subscribers.CountAsync();
            var pageCount = subscribersCount / paginationSettings.PageSize +
                            int.Min(subscribersCount % paginationSettings.PageSize, 1);
            subscribers = subscribers
                .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
                .Take(paginationSettings.PageSize);
            var result = await subscribers.ToListAsync();
            return new SubscriberView(result,
                paginationSettings.PageNumber, pageCount);
        }

        var result2 = await subscribers.ToListAsync();
        return new SubscriberView(result2,
            1, 1);
    }

    private static string CreateEmailBody(string diff)
    {
        var body = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                color: #333;
                                margin: 20px;
                                padding: 0;
                            }}
                            .diff {{
                                white-space: pre;
                            }}
                            .header {{
                                color: #fff;
                                background-color: #2B9ED1;
                                padding: 10px;
                                text-align: center;
                            }}
                            .content {{
                                margin-top: 20px;
                            }}
                            p {{
                                line-height: 1.5;
                            }}
                            .footer {{
                                margin-top: 20px;
                                padding: 10px;
                                background-color: #f0f0f0;
                                text-align: center;
                                font-size: 0.8em;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <h1>New Stock Changes in Our Holdings!</h1>
                        </div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>There are new stock changes in our holdings. Please see below for details:</p>
                            <p class='diff'>{diff}</p>
                            <p>Best regards,</p>
                            <p>Your Quality Soldiers</p>
                        </div>
                        <div class='footer'>
                            Date: {DateTime.Now.ToShortDateString()}
                        </div>
                    </body>
                    </html>
                    ";
        return body;
    }

    private static bool IsEmailAddressValid(string emailAddress)
    {
        try
        {
            var m = new MailAddress(emailAddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
