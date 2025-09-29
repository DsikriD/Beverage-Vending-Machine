namespace BeverageVendingMachine.API.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
    
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
        public List<PaymentCoinDto> PaymentCoins { get; set; } = new List<PaymentCoinDto>(); // Монеты, которые вставил пользователь
    }
    
    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
    
    public class ValidatePaymentDto
    {
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public List<PaymentCoinDto> Coins { get; set; } = new List<PaymentCoinDto>();
    }
    
    public class PaymentCoinDto
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
    }
    
    public class PaymentValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal Change { get; set; }
        public List<ChangeCoinDto>? ChangeCoins { get; set; }
    }
    
    public class ChangeCoinDto
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
    }
}
