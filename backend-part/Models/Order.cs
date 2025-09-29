using System.ComponentModel.DataAnnotations;

namespace BeverageVendingMachine.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? CustomerEmail { get; set; }
        
        [MaxLength(20)]
        public string? CustomerPhone { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    
    public enum OrderStatus
    {
        Pending = 0,
        Paid = 1,
        Processing = 2,
        Completed = 3,
        Cancelled = 4
    }
    
    public enum PaymentMethod
    {
        Cash = 0,
        Card = 1,
        Mobile = 2
    }
}
