using System.ComponentModel.DataAnnotations;
using BeverageVendingMachine.API.Repositories;

namespace BeverageVendingMachine.API.Models
{
    public class Coin : IHasUpdatedAt
    {
        public int Id { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Nominal must be greater than 0")]
        public int Nominal { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Count cannot be negative")]
        public int Count { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsAvailable { get; set; } = true;
    }
}
