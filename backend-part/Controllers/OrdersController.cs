using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly VendingMachineDbContext _context;

        public OrdersController(VendingMachineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                CustomerPhone = o.CustomerPhone,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                CreatedAt = o.CreatedAt,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = 0, // OrderItem больше не связан с Product
                    ProductName = oi.ProductName,
                    BrandName = oi.BrandName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = 0, // OrderItem больше не связан с Product
                    ProductName = oi.ProductName,
                    BrandName = oi.BrandName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPost("validate-payment")]
        public async Task<ActionResult<PaymentValidationResult>> ValidatePayment([FromBody] ValidatePaymentDto validatePaymentDto)
        {
            // Проверяем, что оплачено достаточно
            if (validatePaymentDto.PaidAmount < validatePaymentDto.TotalAmount)
            {
                return Ok(new PaymentValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Недостаточно средств. Требуется: {validatePaymentDto.TotalAmount} руб., внесено: {validatePaymentDto.PaidAmount} руб.",
                    Change = 0
                });
            }

            var change = validatePaymentDto.PaidAmount - validatePaymentDto.TotalAmount;

            if (change == 0)
            {
                return Ok(new PaymentValidationResult
                {
                    IsValid = true,
                    ErrorMessage = null,
                    Change = 0,
                    ChangeCoins = new List<ChangeCoinDto>()
                });
            }

            // Получаем доступные монеты из базы данных
            var availableCoins = await _context.Coins
                .Where(c => c.Count > 0)
                .OrderByDescending(c => c.Nominal)
                .ToListAsync();

            // Алгоритм выдачи сдачи (жадный алгоритм)
            var changeCoins = new List<ChangeCoinDto>();
            var remainingChange = (int)change;

            foreach (var coin in availableCoins)
            {
                if (remainingChange <= 0) break;

                var neededCoins = remainingChange / coin.Nominal;
                var availableCoinsCount = Math.Min(neededCoins, coin.Count);

                if (availableCoinsCount > 0)
                {
                    changeCoins.Add(new ChangeCoinDto
                    {
                        Denomination = coin.Nominal,
                        Quantity = availableCoinsCount
                    });
                    remainingChange -= availableCoinsCount * coin.Nominal;
                }
            }

            // Если не удалось выдать всю сдачу
            if (remainingChange > 0)
            {
                return Ok(new PaymentValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Невозможно выдать сдачу {change} руб. Недостаточно монет нужного номинала. Недостает: {remainingChange} руб.",
                    Change = change
                });
            }

            return Ok(new PaymentValidationResult
            {
                IsValid = true,
                ErrorMessage = null,
                Change = change,
                ChangeCoins = changeCoins
            });
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate products exist and are available
                var productIds = createOrderDto.OrderItems.Select(oi => oi.ProductId).ToList();

                var products = await _context.Products
                    .Include(p => p.Brand) // Включаем связанную сущность Brand
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p);

                foreach (var orderItemDto in createOrderDto.OrderItems)
                {
                    if (!products.ContainsKey(orderItemDto.ProductId))
                    {
                        return BadRequest($"Product with ID {orderItemDto.ProductId} not found");
                    }

                    var product = products[orderItemDto.ProductId];
                    if (!product.IsAvailable || product.StockQuantity < orderItemDto.Quantity)
                    {
                        return BadRequest($"Product {product.Name} is not available in requested quantity");
                    }
                }

                // Create order
                var order = new Order
                {
                    CustomerName = createOrderDto.CustomerName,
                    CustomerEmail = createOrderDto.CustomerEmail,
                    CustomerPhone = createOrderDto.CustomerPhone,
                    PaymentMethod = createOrderDto.PaymentMethod,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Create order items and calculate total
                decimal totalAmount = 0;
                foreach (var orderItemDto in createOrderDto.OrderItems)
                {
                    var product = products[orderItemDto.ProductId];

                    var orderItem = new OrderItem
                    {
                        ProductName = product.Name,
                        BrandName = product.Brand?.Name ?? "Неизвестный бренд", // Проверяем на null
                        Quantity = orderItemDto.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * orderItemDto.Quantity
                    };

                    totalAmount += orderItem.TotalPrice;
                    order.OrderItems.Add(orderItem);

                    // Update stock quantity
                    product.StockQuantity -= orderItemDto.Quantity;
                    product.IsAvailable = product.StockQuantity > 0;
                    product.UpdatedAt = DateTime.UtcNow;
                }

                order.TotalAmount = totalAmount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Добавляем монеты в автомат (пользователь "вставил" монеты)
                await AddCoinsToMachine(createOrderDto);

                // Сохраняем изменения монет
                await _context.SaveChangesAsync();

                // Return created order
                var createdOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstAsync(o => o.Id == order.Id);

                var orderDto = new OrderDto
                {
                    Id = createdOrder.Id,
                    CustomerName = createdOrder.CustomerName,
                    CustomerEmail = createdOrder.CustomerEmail,
                    CustomerPhone = createdOrder.CustomerPhone,
                    TotalAmount = createdOrder.TotalAmount,
                    Status = createdOrder.Status,
                    PaymentMethod = createdOrder.PaymentMethod,
                    CreatedAt = createdOrder.CreatedAt,
                    OrderItems = createdOrder.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = 0, // OrderItem больше не связан с Product
                        ProductName = oi.ProductName,
                        BrandName = oi.BrandName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice
                    }).ToList()
                };

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task AddCoinsToMachine(CreateOrderDto createOrderDto)
        {
            foreach (var paymentCoin in createOrderDto.PaymentCoins)
            {
                var coin = await _context.Coins.FirstOrDefaultAsync(c => c.Nominal == paymentCoin.Denomination);
                if (coin != null)
                {
                    coin.Count += paymentCoin.Quantity;
                    coin.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        private async Task DeductCoinsFromMachine(decimal changeAmount)
        {
            if (changeAmount <= 0)
            {
                return;
            }

            var availableCoins = await _context.Coins
                .Where(c => c.Count > 0)
                .OrderByDescending(c => c.Nominal)
                .ToListAsync();

            var remainingChange = (int)changeAmount;
            var coinsToDeduct = new List<(int denomination, int quantity)>();

            // Алгоритм выдачи сдачи
            foreach (var coin in availableCoins)
            {
                if (remainingChange <= 0) break;

                var neededCoins = remainingChange / coin.Nominal;
                var availableCoinsCount = Math.Min(neededCoins, coin.Count);

                if (availableCoinsCount > 0)
                {
                    coinsToDeduct.Add((coin.Nominal, availableCoinsCount));
                    remainingChange -= availableCoinsCount * coin.Nominal;
                }
            }

            // Вычитаем монеты из автомата
            foreach (var (denomination, quantity) in coinsToDeduct)
            {
                var coin = availableCoins.First(c => c.Nominal == denomination);
                coin.Count -= quantity;
                coin.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto updateOrderStatusDto)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = updateOrderStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Restore stock quantities
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Name == orderItem.ProductName && p.Brand.Name == orderItem.BrandName);
                if (product != null)
                {
                    product.StockQuantity += orderItem.Quantity;
                    product.IsAvailable = product.StockQuantity > 0;
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
