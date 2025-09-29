using Microsoft.AspNetCore.Mvc;
using BeverageVendingMachine.API.Services;
using BeverageVendingMachine.API.Services.Excel;
using BeverageVendingMachine.API.Models.DTOs;

namespace BeverageVendingMachine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ExcelImportService _excelImportService;

        public ImportController(IProductService productService, ExcelImportService excelImportService)
        {
            _productService = productService;
            _excelImportService = excelImportService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            var fileName = file.FileName.ToLower();
            bool isExcel = fileName.EndsWith(".xlsx") || fileName.EndsWith(".xls");
            bool isCsv = fileName.EndsWith(".csv");

            if (!isExcel && !isCsv)
            {
                return BadRequest("Поддерживаются только файлы Excel (.xlsx, .xls) и CSV (.csv)");
            }

            try
            {
                using var stream = file.OpenReadStream();
                List<ImportProductDto> importProducts;

                if (isExcel)
                {
                    importProducts = await _excelImportService.ImportProductsFromExcelAsync(stream);
                }
                else
                {
                    importProducts = await _excelImportService.ImportProductsFromCsvAsync(stream);
                }
                
                if (importProducts.Count == 0)
                {
                    return BadRequest("В файле не найдены валидные продукты");
                }

                var createdProducts = await _productService.ImportProductsAsync(importProducts);

                return Ok(new
                {
                    message = $"Успешно импортировано {createdProducts.Count} продуктов",
                    products = createdProducts
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"Ошибка обработки файла: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpGet("template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var templateBytes = await _excelImportService.CreateExcelTemplateAsync();
                
                return File(templateBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "products_template.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка создания шаблона: {ex.Message}");
            }
        }
    }
}
