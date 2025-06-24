using Surveillance.API.Models;
using System.Collections.Generic;

namespace Surveillance.API.Repositories
{
    public interface ICameraRepository
    {
        IEnumerable<Camera> GetAll();
        Camera? GetById(int id);
        void Add(Camera camera);
        void Update(Camera camera);
        void Delete(int id);
    }
} 