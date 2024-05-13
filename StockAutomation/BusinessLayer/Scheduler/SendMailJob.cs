using BusinessLayer.Facades;
using Quartz;

namespace BusinessLayer.Scheduler;

public class SendMailJob(IProcessDiffFacade facade) : IJob
{
    public static JobKey JobKey { get; } = new("SendMailJob");
    public static TriggerKey TriggerKey { get; } = new("SendMailJob-trigger");

    public async Task Execute(IJobExecutionContext context)
    {
        await facade.ProcessSendDiffLatest();
    }
}