using Surveillance.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace Surveillance.API.Repositories
{
    public class InMemoryCameraRepository : ICameraRepository
    {
        private readonly List<Camera> _cameras = new();
        public IEnumerable<Camera> GetAll() => _cameras;
        public Camera? GetById(int id) => _cameras.FirstOrDefault(c => c.Id == id);
        public void Add(Camera camera) => _cameras.Add(camera);
        public void Update(Camera camera)
        {
            var idx = _cameras.FindIndex(c => c.Id == camera.Id);
            if (idx >= 0) _cameras[idx] = camera;
        }
        public void Delete(int id)
        {
            var cam = _cameras.FirstOrDefault(c => c.Id == id);
            if (cam != null) _cameras.Remove(cam);
        }
    }
} 