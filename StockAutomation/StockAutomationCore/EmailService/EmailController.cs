using StockAutomationCore.Configuration;

namespace StockAutomationCore.EmailService;
using System.Net;
using System.Net.Mail;


public class EmailController
{
    public List<Subscription> Subscriptions { get; set; } = [];

    public void AddSubscriber(string address)
    {
        var newSub = new Subscription { EmailAddress = address};
        Subscriptions.Add(newSub);
    }

    public void RemoveSubscribers(IEnumerable<Subscription> subscriptions)
    {
        foreach (var sub in subscriptions)
        {
            Subscriptions.Remove(sub);
        }
    }

    public void SendEmail(string body)
    {
        var smtpConfig = StockAutomationConfig.GetSection("SMTP");
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
            throw new ArgumentNullException();
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
            Body = CreateEmailBody(body),
            IsBodyHtml = true,
        };

        foreach (var subscription in Subscriptions)
        {
            mailMessage.Bcc.Add(subscription.EmailAddress);
        }

        smtpClient.Send(mailMessage);
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
                            <p>{diff}</p> <!-- You can use more HTML to format the actual body content -->
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
}
