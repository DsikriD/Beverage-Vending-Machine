using BeverageVendingMachine.API.Services;
using System.Text.Json;

namespace BeverageVendingMachine.API.Middleware
{
    public class MachineLockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMachineLockService _machineLockService;

        public MachineLockMiddleware(RequestDelegate next, IMachineLockService machineLockService)
        {
            _next = next;
            _machineLockService = machineLockService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Проверяем только API запросы
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                // Исключаем некоторые эндпоинты из проверки
                var path = context.Request.Path.Value?.ToLower();
                if (ShouldSkipCheck(path))
                {
                    await _next(context);
                    return;
                }

                // Проверяем, есть ли заголовок с ConnectionId (можно передать через фронтенд)
                var connectionId = context.Request.Headers["X-Connection-Id"].FirstOrDefault();
                
                if (string.IsNullOrEmpty(connectionId))
                {
                    // Если нет ConnectionId, проверяем общее состояние автомата
                    if (_machineLockService.IsMachineOccupied())
                    {
                        await ReturnMachineBusyResponse(context);
                        return;
                    }
                }
                else
                {
                    // Если есть ConnectionId, проверяем, является ли этот пользователь активным
                    if (_machineLockService.IsMachineOccupied() && !_machineLockService.IsUserConnected(connectionId))
                    {
                        await ReturnMachineBusyResponse(context);
                        return;
                    }
                }
            }

            await _next(context);
        }

        private static bool ShouldSkipCheck(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            // Пропускаем проверку для определенных эндпоинтов
            var skipPaths = new[]
            {
                "/api/import/products", // Импорт продуктов (админ функция)
                "/api/orders", // GET запросы для просмотра заказов
                "/api/coins" // Управление монетами (админ функция)
            };

            return skipPaths.Any(skipPath => path.StartsWith(skipPath));
        }

        private static async Task ReturnMachineBusyResponse(HttpContext context)
        {
            context.Response.StatusCode = 423; // Locked
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Machine is currently occupied",
                message = "Извините, в данный момент автомат занят другим пользователем",
                code = "MACHINE_OCCUPIED"
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
