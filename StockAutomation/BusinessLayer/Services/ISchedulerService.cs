using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISchedulerService
{
    public Task RescheduleJob(EmailSchedule schedule);
    public Task<EmailSchedule> GetSchedule();
    public Task ScheduleJob();
}
