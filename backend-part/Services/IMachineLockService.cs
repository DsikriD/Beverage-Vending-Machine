namespace BeverageVendingMachine.API.Services
{
    public interface IMachineLockService
    {
        bool IsMachineOccupied();
        string? GetCurrentUser();
        bool IsUserConnected(string connectionId);
        void SetMachineOccupied(string connectionId);
        void SetMachineAvailable();
        void RemoveConnection(string connectionId);
    }
}
