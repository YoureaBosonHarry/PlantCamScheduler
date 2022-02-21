namespace PlantCamScheduler.Models
{
    public class Snapshot
    {
        public Guid SnapshotUUID { get; set; }
        public Guid CameraUUID { get; set; }
        public Guid PlantUUID { get; set; }
        public string SnapshotData { get; set; }
        public string? SnapshotMetadata { get; set; }
    }
}
