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
    public async Task<Result<EmailSchedule, Error>> GetSchedule()
    {
        var schedule = await context.EmailSchedules.FirstOrDefaultAsync();
        if (schedule == null)
        {
            return new Error
            {
                ErrorType = ErrorType.SchedulerError,
                Message = "No schedule found"
            };
        }

        return schedule;
    }

    public async Task ScheduleJob()
    {
        var schedule = await context.EmailSchedules.FirstOrDefaultAsync();
        if (schedule == null)
        {
            schedule = new EmailSchedule
            {
                Days = 1,
                Hours = 8,
                Minutes = 0
            };
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


    public async Task RescheduleJob(EmailSchedule schedule)
    {
        var scheduleDb = await context.EmailSchedules.FirstOrDefaultAsync(s =>
            s.Days == schedule.Days && s.Hours == schedule.Hours && s.Minutes == schedule.Minutes);
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

        var cronSchedule = GetCronSchedule(schedule);

        var trigger = TriggerBuilder.Create()
            .WithIdentity(SendMailJob.TriggerKey)
            .WithCronSchedule(cronSchedule)
            .Build();

        if (await scheduler.CheckExists(SendMailJob.JobKey))
        {
            await scheduler.RescheduleJob(SendMailJob.TriggerKey, trigger);
            Console.WriteLine("Job exists");
            return;
        }

        Console.WriteLine("Job does not exist");

        var jobDetail = JobBuilder.Create<SendMailJob>()
            .WithIdentity(SendMailJob.JobKey)
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger);
    }

    private string GetCronSchedule(EmailSchedule schedule)
    {
        return $"0 {schedule.Minutes} {schedule.Hours} ? * 1/{schedule.Days} *";
    }
}
