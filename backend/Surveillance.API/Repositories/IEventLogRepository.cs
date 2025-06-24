using Surveillance.API.Models;
using System.Collections.Generic;

namespace Surveillance.API.Repositories
{
    public interface IEventLogRepository
    {
        IEnumerable<EventLog> GetAll();
        void Add(EventLog log);
    }
} 