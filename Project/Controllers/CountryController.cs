using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project.Models; // Assuming you have your DTOs and Entities
using Business.Helpers;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; // For ApplicationDbContext

namespace Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _restCountriesUrl;
        private readonly IMemoryCache _memoryCache;
        private readonly ApplicationDbContext _dbContext;

        public CountryController(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IMemoryCache memoryCache, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _restCountriesUrl = apiSettings?.Value?.RestCountriesUrl ?? throw new ArgumentNullException("RestCountriesUrl is not configured.");
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // Now DbContext is injected
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet]
        public async Task<IActionResult> GetCountriesAsync()
        {
            try
            {
                var countryHelper = new CountryHelper(_httpClient, _dbContext, _restCountriesUrl, _memoryCache);

                List<CountryDTO> countries = await countryHelper.FetchCountriesAsync();

                if (countries == null || countries.Count == 0)
                {
                    return BadRequest(new { message = "No countries found." });
                }

                return Ok(countries); // Return the list of countries
                

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
