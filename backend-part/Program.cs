using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Repositories;
using BeverageVendingMachine.API.Repositories.Brand;
using BeverageVendingMachine.API.Repositories.Product;
using BeverageVendingMachine.API.Repositories.Order;
using BeverageVendingMachine.API.Repositories.Coin;
using BeverageVendingMachine.API.Services;
using BeverageVendingMachine.API.Services.Excel;
using BeverageVendingMachine.API.Hubs;
using BeverageVendingMachine.API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Entity Framework
builder.Services.AddDbContext<VendingMachineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICoinRepository, CoinRepository>();

// Add Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ExcelImportService>();
builder.Services.AddSingleton<IMachineLockService, MachineLockService>();

// Add SignalR
builder.Services.AddSignalR();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                           ?? new[] { "http://localhost:3000", "https://localhost:3000" };
        
        var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() 
                           ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        
        var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() 
                           ?? new[] { "*" };
        
        policy.WithOrigins(allowedOrigins)
              .WithMethods(allowedMethods)
              .WithHeaders(allowedHeaders)
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true); // Для SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Add machine lock middleware before authorization
app.UseMiddleware<MachineLockMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<VendingMachineHub>("/vendingMachineHub");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VendingMachineDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
