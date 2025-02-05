using API.Helpers;
using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Đăng ký dịch vụ 
builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>))); // Đăng ký dịch vụ GenericRepository
builder.Services.AddAutoMapper(typeof(MappingProfiles)); // Đăng ký dịch vụ AutoMapper
builder.Services.AddControllers(); // Đăng ký dịch vụ Controllers

builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>(); // Sử dụng middleware xử lý lỗi toàn cục
app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Điều hướng các lỗi sang controller ErrorsController

app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

// Tạo scope để truy cập dịch vụ từ DI container
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context, loggerFactory);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred during migration");
    }
}


app.Run();
