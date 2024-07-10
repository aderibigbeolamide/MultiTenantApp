using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using YourNamespace.Data; // Replace with your actual namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Finbuckle.MultiTenant services and configure the store
builder.Services.AddMultiTenant<TenantInfo>()
    .WithConfigurationStore()
    .WithRouteStrategy();

// Configure the DbContext for tenant databases
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var tenantInfo = serviceProvider.GetService<ITenantInfo>();
    if (tenantInfo != null)
    {
        options.UseMySql(tenantInfo.ConnectionString, ServerVersion.AutoDetect(tenantInfo.ConnectionString));
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use Finbuckle MultiTenant
app.UseMultiTenant();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
