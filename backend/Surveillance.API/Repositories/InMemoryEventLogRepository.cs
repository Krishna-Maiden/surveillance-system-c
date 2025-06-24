using Surveillance.API.Models;
using System.Collections.Generic;

namespace Surveillance.API.Repositories
{
    public class InMemoryEventLogRepository : IEventLogRepository
    {
        private readonly List<EventLog> _logs = new();
        public IEnumerable<EventLog> GetAll() => _logs;
        public void Add(EventLog log) => _logs.Add(log);
    }
} 