namespace PlantCamScheduler.Models
{
    public class SnapshotScheduleResponse
    {
        public SnapshotScheduleRequest SnapshotRequest { get; set; }
        public AdditionStatus AdditionStatus { get; set; }

    }

    public enum AdditionStatus
    {
        Success,
        DuplicateCameraRequest,
        CameraNotAvailable,
        InternalFailure
    }
}
