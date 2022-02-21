using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using PlantCamScheduler.Models;
using PlantCamScheduler.Services.Interfaces;

namespace PlantCamScheduler.Controllers
{
    [ApiController]
    [Route("scheduler")]
    public class ScheduleController : ControllerBase
    {
        private readonly ISchedulerService frameSchedulerService;
        public ScheduleController(ISchedulerService frameSchedulerService)
        {
            this.frameSchedulerService = frameSchedulerService;
        }

        [HttpPost]
        [Route("addcameratoschedule")]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(SnapshotScheduleResponse))]
        public async Task<IActionResult> AddCameraToSchedulerAsync([FromBody]SnapshotScheduleRequest camera)
        {
            var cameraReponse = this.frameSchedulerService.AddCameraToSnapshotSchedule(camera);
            return Ok(cameraReponse);
        }
    }
}
