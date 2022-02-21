namespace PlantCamScheduler.Models
{
    public class FrameResponse
    {
        public SnapshotStatus SnapshotStatus { get; set; }
    }
    public enum SnapshotStatus
    {
        Success,
        Failure
    }
}
