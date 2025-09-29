using OfficeOpenXml;
using BeverageVendingMachine.API.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BeverageVendingMachine.API.Services.Excel
{
    public class ExcelImportService
    {
        public async Task<List<ImportProductDto>> ImportProductsFromCsvAsync(Stream fileStream)
        {
            var products = new List<ImportProductDto>();
            
            using var reader = new StreamReader(fileStream);
            var lines = new List<string>();
            
            while (!reader.EndOfStream)
            {
                lines.Add(await reader.ReadLineAsync());
            }
            
            if (lines.Count < 2)
                throw new InvalidOperationException("Файл должен содержать заголовки и хотя бы одну строку данных");
            
            // Читаем данные начиная со второй строки (первая - заголовки)
            for (int i = 1; i < lines.Count; i++)
            {
                try
                {
                    var columns = ParseCsvLine(lines[i]);
                    
                    if (columns.Length < 6)
                        throw new InvalidOperationException($"Недостаточно колонок в строке {i + 1}");
                    
                    var product = new ImportProductDto
                    {
                        Name = columns[0]?.Trim() ?? string.Empty,
                        BrandName = columns[1]?.Trim() ?? string.Empty,
                        Price = Convert.ToDecimal(columns[2]),
                        ImageUrl = columns[3]?.Trim(),
                        StockQuantity = Convert.ToInt32(columns[4] ?? "0"),
                        Description = columns[5]?.Trim()
                    };
                    
                    // Валидация
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(product);
                    
                    if (Validator.TryValidateObject(product, validationContext, validationResults, true))
                    {
                        products.Add(product);
                    }
                    else
                    {
                        var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                        throw new InvalidOperationException($"Ошибка валидации в строке {i + 1}: {errors}");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Ошибка обработки строки {i + 1}: {ex.Message}");
                }
            }
            
            return products;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;
            char delimiter = ';'; // Только точка с запятой
            
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == delimiter && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            
            result.Add(current);
            return result.ToArray();
        }

        public async Task<List<ImportProductDto>> ImportProductsFromExcelAsync(Stream fileStream)
        {
            var products = new List<ImportProductDto>();
            
            // Настройка лицензии EPPlus (для некоммерческого использования)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            
            if (worksheet == null)
                throw new InvalidOperationException("Лист не найден в Excel файле");
            
            var rowCount = worksheet.Dimension?.Rows ?? 0;
            
            // Проверяем наличие данных
            if (rowCount < 1)
                throw new InvalidOperationException("Файл должен содержать хотя бы одну строку данных");
            
            // Читаем данные начиная с первой строки (без заголовков)
            for (int row = 1; row <= rowCount; row++)
            {
                try
                {
                    var product = new ImportProductDto
                    {
                        Name = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty,
                        BrandName = worksheet.Cells[row, 2].Text?.Trim() ?? string.Empty,
                        Price = Convert.ToDecimal(worksheet.Cells[row, 3].Text),
                        ImageUrl = worksheet.Cells[row, 4].Text?.Trim(),
                        StockQuantity = Convert.ToInt32(worksheet.Cells[row, 5].Text ?? "0"),
                        Description = worksheet.Cells[row, 6].Text?.Trim()
                    };
                    
                    // Валидация
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(product);
                    
                    if (Validator.TryValidateObject(product, validationContext, validationResults, true))
                    {
                        products.Add(product);
                    }
                    else
                    {
                        var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                        throw new InvalidOperationException($"Ошибка валидации в строке {row}: {errors}");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Ошибка обработки строки {row}: {ex.Message}");
                }
            }
            
            return products;
        }
        
        public async Task<byte[]> CreateExcelTemplateAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Продукты");
            
            // Заголовки (соответствуют CSV формату с точкой с запятой)
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Brand";
            worksheet.Cells[1, 3].Value = "Price";
            worksheet.Cells[1, 4].Value = "Image URL";
            worksheet.Cells[1, 5].Value = "Stock Quantity";
            worksheet.Cells[1, 6].Value = "Description";
            
            // Форматирование заголовков
            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }
            
            // Автоширина колонок
            worksheet.Cells.AutoFitColumns();
            
            return await Task.FromResult(package.GetAsByteArray());
        }
    }
}
