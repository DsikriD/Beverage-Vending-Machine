using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly VendingMachineDbContext _context;

        public ProductsController(VendingMachineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
            [FromQuery] string? brand = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? available = null)
        {
            var query = _context.Products.Include(p => p.Brand).AsQueryable();

            if (!string.IsNullOrEmpty(brand) && brand != "all")
            {
                query = query.Where(p => p.Brand.Name == brand);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (available.HasValue)
            {
                query = query.Where(p => p.IsAvailable == available.Value);
            }

            var products = await query.ToListAsync();

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable,
                StockQuantity = p.StockQuantity
            }).ToList();

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity
            };

            return Ok(productDto);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetBrands()
        {
            var brands = await _context.Brands
                .Where(b => b.Products.Any(p => p.IsAvailable))
                .Select(b => b.Name)
                .ToListAsync();

            return Ok(brands);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == createProductDto.Brand);
            if (brand == null)
            {
                return BadRequest($"Brand '{createProductDto.Brand}' not found");
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                BrandId = brand.Id,
                Price = createProductDto.Price,
                ImageUrl = createProductDto.ImageUrl,
                StockQuantity = createProductDto.StockQuantity,
                IsAvailable = createProductDto.StockQuantity > 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Load brand for response
            await _context.Entry(product).Reference(p => p.Brand).LoadAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (updateProductDto.Name != null)
                product.Name = updateProductDto.Name;
            if (updateProductDto.Brand != null)
            {
                var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == updateProductDto.Brand);
                if (brand == null)
                {
                    return BadRequest($"Brand '{updateProductDto.Brand}' not found");
                }
                product.BrandId = brand.Id;
            }
            if (updateProductDto.Price.HasValue)
                product.Price = updateProductDto.Price.Value;
            if (updateProductDto.ImageUrl != null)
                product.ImageUrl = updateProductDto.ImageUrl;
            if (updateProductDto.IsAvailable.HasValue)
                product.IsAvailable = updateProductDto.IsAvailable.Value;
            if (updateProductDto.StockQuantity.HasValue)
                product.StockQuantity = updateProductDto.StockQuantity.Value;

            product.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
