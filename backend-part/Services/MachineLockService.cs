using System.Collections.Concurrent;

namespace BeverageVendingMachine.API.Services
{
    public class MachineLockService : IMachineLockService
    {
        private static readonly ConcurrentDictionary<string, string> _activeConnections = new();
        private static string? _currentUser = null;
        private static readonly object _lockObject = new();

        public bool IsMachineOccupied()
        {
            lock (_lockObject)
            {
                return _currentUser != null;
            }
        }

        public string? GetCurrentUser()
        {
            lock (_lockObject)
            {
                return _currentUser;
            }
        }

        public bool IsUserConnected(string connectionId)
        {
            lock (_lockObject)
            {
                return _currentUser == connectionId && _activeConnections.ContainsKey(connectionId);
            }
        }

        public void SetMachineOccupied(string connectionId)
        {
            lock (_lockObject)
            {
                if (_currentUser == null)
                {
                    _currentUser = connectionId;
                    _activeConnections.TryAdd(connectionId, connectionId);
                }
            }
        }

        public void SetMachineAvailable()
        {
            lock (_lockObject)
            {
                _currentUser = null;
                _activeConnections.Clear();
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (_lockObject)
            {
                if (_currentUser == connectionId)
                {
                    _currentUser = null;
                }
                _activeConnections.TryRemove(connectionId, out _);
            }
        }

        public static int GetActiveConnectionsCount()
        {
            return _activeConnections.Count;
        }
    }
}
