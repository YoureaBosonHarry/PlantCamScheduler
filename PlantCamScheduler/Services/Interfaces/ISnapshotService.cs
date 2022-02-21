using PlantCamScheduler.Models;

namespace PlantCamScheduler.Services.Interfaces
{
    public interface ISnapshotService
    {
        Task<IEnumerable<Snapshot>> GetSnapshotsByPlantUUIDAsync(Guid plantUUID, int numRows);
    }
}
