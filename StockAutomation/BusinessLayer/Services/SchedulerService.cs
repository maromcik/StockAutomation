using BusinessLayer.Errors;
using BusinessLayer.Scheduler;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace BusinessLayer.Services;

public class SchedulerService(StockAutomationDbContext context, IScheduler scheduler)
    : ISchedulerService
{
    private static readonly EmailSchedule DefaultSchedule = new()
    {
        Days = 1,
        Hours = 8,
        Minutes = 0
    };

    public async Task<EmailSchedule> GetSchedule()
    {
        var schedule = await context.EmailSchedules.FirstOrDefaultAsync() ?? DefaultSchedule;
        return schedule;
    }

    public async Task ScheduleJob()
    {
        var schedule = await context.EmailSchedules.FirstOrDefaultAsync();
        if (schedule == null)
        {
            schedule = DefaultSchedule;
            context.Add(schedule);
            await context.SaveChangesAsync();
        }

        var cronSchedule = GetCronSchedule(schedule);

        var jobDetail = JobBuilder.Create<SendMailJob>()
            .WithIdentity(SendMailJob.JobKey)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(SendMailJob.TriggerKey)
            .WithCronSchedule(cronSchedule)
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger);
    }


    public async Task<Result<bool, Error>> RescheduleJob(EmailSchedule schedule)
    {
        try
        {
            var cronSchedule = GetCronSchedule(schedule);
            var trigger = TriggerBuilder.Create()
                .WithIdentity(SendMailJob.TriggerKey)
                .WithCronSchedule(cronSchedule)
                .Build();

            if (await scheduler.CheckExists(SendMailJob.JobKey))
            {
                await scheduler.RescheduleJob(SendMailJob.TriggerKey, trigger);
                return true;
            }

            var jobDetail = JobBuilder.Create<SendMailJob>()
                .WithIdentity(SendMailJob.JobKey)
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
        }

        catch (Exception e)
        {
            return new Error
            {
                ErrorType = ErrorType.SchedulerError,
                Message = e.Message
            };
        }

        var scheduleDb = await context.EmailSchedules.FirstOrDefaultAsync();
        if (scheduleDb == null)
        {
            context.Add(schedule);
        }
        else
        {
            scheduleDb.Days = schedule.Days;
            scheduleDb.Hours = schedule.Hours;
            scheduleDb.Minutes = schedule.Minutes;
            context.Update(scheduleDb);
        }

        await context.SaveChangesAsync();
        return true;
    }

    private string GetCronSchedule(EmailSchedule schedule)
    {
        var minutes = $"{schedule.Minutes}";
        var hours = $"{schedule.Hours}";
        var days = $"1/{schedule.Days}";
        if (schedule.Days is 0 or 1)
        {
            days = "*";
        }

        var cron = $"0 {minutes} {hours} ? * {days} *";
        Console.WriteLine(cron);
        return cron;
    }
}
