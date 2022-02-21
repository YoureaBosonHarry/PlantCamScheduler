using PlantCamScheduler.Models;
using PlantCamScheduler.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace PlantCamScheduler.Services
{
    public class SchedulerService: ISchedulerService
    {
        private  Dictionary<Guid, DateTime> snapshotExecutionSchedule = new Dictionary<Guid, DateTime>();
        private List<SnapshotScheduleRequest> snapshotRequestList = new List<SnapshotScheduleRequest>();
        private readonly string cameraServiceEndpoint;
        private Serilog.ILogger log;
        private Thread snapshotExecutionThread;
        private bool isRunning = false;
        private HttpClient httpClient = new HttpClient();

        public SchedulerService(string cameraServiceEndpoint)
        {
            this.cameraServiceEndpoint = cameraServiceEndpoint;
            this.log = Serilog.Log.ForContext<SchedulerService>();
            
        }

        public async Task StartSnapshotExecutionThreadAsync()
        {
            isRunning = true;
            snapshotExecutionThread = new Thread(async () => await TakeSnapshotAsync());
            snapshotExecutionThread.Start();
        }

        public async Task<SnapshotScheduleResponse> AddCameraToSnapshotSchedule(SnapshotScheduleRequest snapshotRequest)
        {
            try
            {
                snapshotRequestList.Add(snapshotRequest);
                var cameraAdded = snapshotExecutionSchedule.TryAdd(snapshotRequest.CameraUUID, DateTime.UtcNow);
                if (!cameraAdded)
                {
                    this.log.Warning($"Camera cannot be added to snapshot schedule");
                    return new SnapshotScheduleResponse { SnapshotRequest = snapshotRequest, AdditionStatus = AdditionStatus.DuplicateCameraRequest };
                }
                if (!isRunning)
                {
                    await this.StartSnapshotExecutionThreadAsync();
                }
                return new SnapshotScheduleResponse { SnapshotRequest = snapshotRequest, AdditionStatus = AdditionStatus.Success }; 
            }
            catch (Exception ex)
            {
                this.log.Error($"Error adding camera to snapshot schedule: {ex.Message}");
                return new SnapshotScheduleResponse { SnapshotRequest = snapshotRequest, AdditionStatus= AdditionStatus.InternalFailure };
            }
        } 

        private async Task TakeSnapshotAsync()
        {
            while (isRunning)
            {
                foreach (var snapshotUUID in snapshotExecutionSchedule.Keys)
                {
                    if (snapshotExecutionSchedule[snapshotUUID] <= DateTime.UtcNow)
                    {
                        var selectedSnapshot = snapshotRequestList.Where(i => i.CameraUUID == snapshotUUID).SingleOrDefault();
                        var snapshotRequest = new FrameRequest { CameraUUID = snapshotUUID, PlantUUID = selectedSnapshot.PlantUUID };
                        this.log.Information($"Sending snapshot request for: {snapshotUUID} at time: {snapshotExecutionSchedule[snapshotUUID]}");
                        var snapshotResponse = await this.SendSnapshotRequestAsync(snapshotRequest);
                        if (snapshotResponse.SnapshotStatus == SnapshotStatus.Success)
                        {
                            snapshotExecutionSchedule[snapshotUUID] = DateTime.UtcNow + TimeSpan.FromSeconds(selectedSnapshot.SnapshotDelay);
                            this.log.Information($"Snapshot successful for: {snapshotUUID}. Next snapshot scheduled for: {snapshotExecutionSchedule[snapshotUUID]}");
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private async Task<FrameResponse> SendSnapshotRequestAsync(FrameRequest snapshotRequest)
        {
            try
            {
                var serilizedSnapshotRequest = JsonSerializer.Serialize<FrameRequest>(snapshotRequest);
                var stringContent = new StringContent(serilizedSnapshotRequest, Encoding.Default, "application/json");
                var response = await httpClient.PostAsync($"{this.cameraServiceEndpoint}/TakePhoto", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    return new FrameResponse { SnapshotStatus = SnapshotStatus.Success };
                }
                return new FrameResponse { SnapshotStatus = SnapshotStatus.Failure };
            }
            catch (Exception ex)
            {
                this.log.Error($"Unhandled Exception: {ex.Message}");
                return new FrameResponse { SnapshotStatus = SnapshotStatus.Failure };
            }
        }

    }
}
