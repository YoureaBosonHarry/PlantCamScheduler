using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlantCamScheduler.Models;
using PlantCamScheduler.Services.Interfaces;

namespace PlantCamScheduler.Controllers
{
    [Route("snapshot")]
    [ApiController]
    public class SnapshotController : ControllerBase
    {
        private readonly ISnapshotService snapshotService;
        public SnapshotController(ISnapshotService snapshotService)
        {
            this.snapshotService = snapshotService;
        }

        [HttpGet("getsnapshotsbyuuid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Snapshot>))]
        public async Task<IActionResult> GetSnapshotsByPlantUUIDAsync([FromQuery] Guid plantUUID, [FromQuery] int numRows)
        {
            var snapshots = this.snapshotService.GetSnapshotsByPlantUUIDAsync(plantUUID, numRows);
            return Ok(snapshots);
        }
    }
}
