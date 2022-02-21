namespace PlantCamScheduler.Models
{
    public class SnapshotScheduleRequest
    {
        public Guid CameraUUID { get; set; }
        public Guid PlantUUID { get; set; }
        public int SnapshotDelay { get; set; }
        public string? SnapshotMetadata { get; set; }
    }
}
