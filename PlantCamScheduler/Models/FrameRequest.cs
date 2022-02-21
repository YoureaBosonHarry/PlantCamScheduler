namespace PlantCamScheduler.Models
{
    public class FrameRequest
    {
        public Guid CameraUUID { get; set; }
        public Guid PlantUUID { get; set; }
        public string? SnapshotMetadata { get; set; }
    }
}
