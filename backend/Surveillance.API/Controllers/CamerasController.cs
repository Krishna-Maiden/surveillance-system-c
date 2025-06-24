using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;
using Surveillance.API.Repositories;

namespace Surveillance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CamerasController : ControllerBase
    {
        private readonly ICameraRepository _repo;
        public CamerasController(ICameraRepository repo) => _repo = repo;

        [HttpGet]
        public ActionResult<IEnumerable<Camera>> Get() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public ActionResult<Camera> Get(int id)
        {
            var cam = _repo.GetById(id);
            return cam == null ? NotFound() : Ok(cam);
        }

        [HttpPost]
        public IActionResult Post(Camera camera)
        {
            _repo.Add(camera);
            return CreatedAtAction(nameof(Get), new { id = camera.Id }, camera);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Camera updated)
        {
            var cam = _repo.GetById(id);
            if (cam == null) return NotFound();
            _repo.Update(updated);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cam = _repo.GetById(id);
            if (cam == null) return NotFound();
            _repo.Delete(id);
            return NoContent();
        }
    }
} 