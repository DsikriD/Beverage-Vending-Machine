using System.ComponentModel.DataAnnotations;

namespace BeverageVendingMachine.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        // Неизменяемые данные на момент заказа (согласно ТЗ)
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string BrandName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
    }
}
