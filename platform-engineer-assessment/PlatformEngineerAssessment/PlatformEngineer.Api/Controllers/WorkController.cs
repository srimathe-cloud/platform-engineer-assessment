using Microsoft.AspNetCore.Mvc;
using PlatformEngineer.Api.Models;
using PlatformEngineer.Api.Services;

namespace PlatformEngineer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkController : ControllerBase
    {
        private readonly WorkService _WorkService;

        public WorkController(WorkService workService)
        {
            _WorkService = workService;
        }

        // Returns all items (queued + processed)
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_WorkService.GetAllItems());
        }

        // Returns only queued (pending) items
        [HttpGet("queue")]
        public IActionResult GetQueue()
        {
            return Ok(_WorkService.GetQueuedItems());
        }

        // Returns processed items
        [HttpGet("processed")]
        public IActionResult GetProcessed()
        {
            return Ok(_WorkService.GetProcessedItems());
        }

        // Enqueue work for background processing
        [HttpPost]
        public IActionResult EnqueueWork([FromBody] WorkItem work)
        {
            var item = _WorkService.Enqueue(work);

            // Return 202 Accepted for queued work
            return Accepted(item);
        }
    }
}