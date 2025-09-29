using System.ComponentModel.DataAnnotations;

namespace BeverageVendingMachine.API.Models.DTOs
{
    public class ImportProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string BrandName { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int StockQuantity { get; set; } = 0;
        
        public string? Description { get; set; }
    }
}
