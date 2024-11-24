using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Business.Helpers;
using Project.Models;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext to the services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Project"))); // Use connection string from appsettings.json

// Register the ApiSettings configuration section from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register HttpClient for API calls (this is crucial for CountryHelper)
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache(); // Adds IMemoryCache

// Register CountryHelper with HttpClient and configuration from ApiSettings
builder.Services.AddScoped(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>(); // Get HttpClient instance
    var dbContext = sp.GetRequiredService<ApplicationDbContext>(); // Get DbContext instance
    var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>(); // Get configuration values
    var memoryCache = sp.GetRequiredService<IMemoryCache>(); // Get IMemoryCache instance

    return new CountryHelper(httpClient, dbContext, apiSettings?.RestCountriesUrl, memoryCache); // Pass HttpClient and the API URL to CountryHelper
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

app.Run();
