using BusinessLayer.Facades;
using Quartz;

namespace BusinessLayer.Scheduler;

public class SendMailJob(ISendDifferencesFacade facade) : IJob
{
    public static JobKey JobKey { get; }= new("SendMailJob");
    public static TriggerKey TriggerKey { get; }= new("SendMailJob-trigger");
    public async Task Execute(IJobExecutionContext context)
    {
        await facade.ProcessDiffLatest();
    }
}
