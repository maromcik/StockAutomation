using BusinessLayer.Facades;
using Quartz;

namespace StockAutomationWeb.Scheduler;

public class SendMailJob(ISendDifferencesFacade facade) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await facade.ProcessDiffLatest();
    }
}
