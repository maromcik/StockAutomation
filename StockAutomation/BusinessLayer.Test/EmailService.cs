// using System.Transactions;
using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Test;

public class EmailServiceTests
{
    private DbContextOptions _options;
    // private TransactionScope _transaction;

    [SetUp]
    public void Setup()
    {
        var uniqueDbName = Guid.NewGuid().ToString();

        _options = new DbContextOptionsBuilder().UseInMemoryDatabase(uniqueDbName).Options;
    }

    [Test]
    public void GetEmailSettings_EmptyDB_ReturnsDefault()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Dereference of a possibly null reference.
        var service = new EmailService(context, null /* will not use config */);
#pragma warning restore CS8625 // Dereference of a possibly null reference.

        // Act

        var emailSettings = service.GetEmailSettings().Result;

        // Assert

        Assert.That(emailSettings, Is.Not.EqualTo(null));
    }

    [Test]
    public async Task SaveEmailSettingsAsync_AnyDBState_SettingsPersisted()
    {
        // Arrange

        var context = new StockAutomationDbContext(_options);

#pragma warning disable CS8625 // Dereference of a possibly null reference.
        var service = new EmailService(context, null /* will not use config */);
#pragma warning restore CS8625 // Dereference of a possibly null reference.

        // Act

        var emailSettings = service.GetEmailSettings().Result;

        var saveHtmlOutput = await service.SaveEmailSettingsAsync(new FormatSettings(DataAccessLayer.Entities.OutputFormat.HTML));
        var contentAfterSaveHtml = await service.GetEmailSettings();

        var saveJsonOutput = await service.SaveEmailSettingsAsync(new FormatSettings(DataAccessLayer.Entities.OutputFormat.JSON));
        var contentAfterSaveJson = await service.GetEmailSettings();

        var saveTextOutput = await service.SaveEmailSettingsAsync(new FormatSettings(DataAccessLayer.Entities.OutputFormat.Text));
        var contentAfterSaveText = await service.GetEmailSettings();

        Assert.Multiple(() =>
        {

            // Assert

            Assert.That(emailSettings, Is.Not.EqualTo(null));

            Assert.That(saveHtmlOutput.IsOk);
            Assert.That(contentAfterSaveHtml.PreferredFormat, Is.EqualTo(DataAccessLayer.Entities.OutputFormat.HTML));

            Assert.That(saveJsonOutput.IsOk);
            Assert.That(contentAfterSaveJson.PreferredFormat, Is.EqualTo(DataAccessLayer.Entities.OutputFormat.JSON));

            Assert.That(saveTextOutput.IsOk);
            Assert.That(contentAfterSaveText.PreferredFormat, Is.EqualTo(DataAccessLayer.Entities.OutputFormat.Text));
        });
    }
}
