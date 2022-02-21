using PlantCamScheduler.Models;
using PlantCamScheduler.Services.Interfaces;
using Serilog;
using System.Text.Json;

namespace PlantCamScheduler.Services
{
    public class SnapshotService: ISnapshotService
    {
        private readonly string cameraServiceEndpoint;
        private Serilog.ILogger log;
        private readonly HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(3)};
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true};
        public SnapshotService(string cameraServiceEndpoint)
        {
            this.cameraServiceEndpoint = cameraServiceEndpoint;
            this.log = Log.ForContext<SnapshotService>();
        }

        public async Task<IEnumerable<Snapshot>> GetSnapshotsByPlantUUIDAsync(Guid plantUUID, int numRows)
        {
            this.log.Information($"Sending request for {numRows} snapshots of plant: {plantUUID}");
            var unencodedUrl = $"{this.cameraServiceEndpoint}/GetSnapshotsByUUID?plantUUID={plantUUID}&numRows={numRows}";
            var encodedUrl = Uri.EscapeDataString(unencodedUrl);
            var response = await httpClient.GetAsync(encodedUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var snapshotList = await JsonSerializer.DeserializeAsync<IEnumerable<Snapshot>>(responseStream);
                return snapshotList;
            }
            return new List<Snapshot>();
        }
    }
}
