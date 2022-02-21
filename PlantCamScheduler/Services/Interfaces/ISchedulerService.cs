using PlantCamScheduler.Models;

namespace PlantCamScheduler.Services.Interfaces
{
    public interface ISchedulerService
    {
        Task StartSnapshotExecutionThreadAsync();
        Task<SnapshotScheduleResponse> AddCameraToSnapshotSchedule(SnapshotScheduleRequest camera);
    }
}
