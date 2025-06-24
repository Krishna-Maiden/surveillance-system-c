using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;
using Surveillance.API.Repositories;

namespace Surveillance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventLogsController : ControllerBase
    {
        private readonly IEventLogRepository _repo;
        public EventLogsController(IEventLogRepository repo) => _repo = repo;

        [HttpGet]
        public ActionResult<IEnumerable<EventLog>> Get() => Ok(_repo.GetAll());

        [HttpPost]
        public IActionResult Post(EventLog log)
        {
            _repo.Add(log);
            return CreatedAtAction(nameof(Get), new { id = log.Id }, log);
        }
    }
} 