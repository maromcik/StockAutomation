using BusinessLayer.Errors;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services;

public interface ISchedulerService
{
    public Task<Result<bool, Error>> RescheduleJob(EmailSchedule schedule);
    public Task<EmailSchedule> GetSchedule();
    public Task ScheduleJob();
}
