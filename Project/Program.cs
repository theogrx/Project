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

// Register the ApiSettings configuration section from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register HttpClient for API calls (this is crucial for CountryHelper)
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

app.Run();
