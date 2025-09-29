using Microsoft.AspNetCore.SignalR;
using BeverageVendingMachine.API.Services;

namespace BeverageVendingMachine.API.Hubs
{
    public class VendingMachineHub : Hub
    {
        private readonly IMachineLockService _machineLockService;

        public VendingMachineHub(IMachineLockService machineLockService)
        {
            _machineLockService = machineLockService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            // Если автомат свободен, новый пользователь занимает его
            if (!_machineLockService.IsMachineOccupied())
            {
                _machineLockService.SetMachineOccupied(Context.ConnectionId);
                
                // Уведомляем текущего пользователя, что автомат доступен для него
                await Clients.Caller.SendAsync("MachineAvailable");
                
                // Уведомляем всех остальных клиентов, что автомат занят
                await Clients.Others.SendAsync("MachineOccupied", Context.ConnectionId);
            }
            else
            {
                // Автомат уже занят, уведомляем нового пользователя
                await Clients.Caller.SendAsync("MachineBusy", "Извините, в данный момент автомат занят");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Если отключился текущий пользователь, освобождаем автомат
            if (_machineLockService.IsUserConnected(Context.ConnectionId))
            {
                _machineLockService.RemoveConnection(Context.ConnectionId);
                _machineLockService.SetMachineAvailable();
                
                // Уведомляем всех клиентов, что автомат свободен
                await Clients.All.SendAsync("MachineAvailable");
            }
            else
            {
                // Удаляем из активных соединений
                _machineLockService.RemoveConnection(Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // Метод для проверки статуса автомата
        public async Task CheckMachineStatus()
        {
            if (!_machineLockService.IsMachineOccupied())
            {
                // Автомат свободен
                await Clients.Caller.SendAsync("MachineAvailable");
            }
            else if (_machineLockService.IsUserConnected(Context.ConnectionId))
            {
                // Текущий пользователь - автомат доступен для него
                await Clients.Caller.SendAsync("MachineAvailable");
            }
            else
            {
                // Автомат занят другим пользователем
                await Clients.Caller.SendAsync("MachineBusy", "Извините, в данный момент автомат занят");
            }
        }

        // Метод для принудительного освобождения автомата (для админа)
        public async Task ForceReleaseMachine()
        {
            if (_machineLockService.IsMachineOccupied())
            {
                var previousUser = _machineLockService.GetCurrentUser();
                _machineLockService.SetMachineAvailable();
                
                // Уведомляем предыдущего пользователя
                if (!string.IsNullOrEmpty(previousUser))
                {
                    await Clients.Client(previousUser).SendAsync("MachineForceReleased", "Автомат был освобожден администратором");
                }
                
                // Уведомляем всех, что автомат свободен
                await Clients.All.SendAsync("MachineAvailable");
            }
        }
    }
}
