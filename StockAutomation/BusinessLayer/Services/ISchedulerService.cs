using BusinessLayer.Errors;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISchedulerService
{
    public Task RescheduleJob(EmailSchedule schedule);
    public Task<Result<EmailSchedule, Error>> GetSchedule();
    public Task ScheduleJob();
}
