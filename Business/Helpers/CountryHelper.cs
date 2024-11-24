using System.Diagnostics.Metrics;
using System.Text.Json;
using Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Project.Models;
namespace Business.Helpers
{
    public class CountryHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly string _restCountriesUrl;
        private readonly IMemoryCache _memoryCache;
        private bool _disposed = false;

        public CountryHelper(HttpClient httpClient, ApplicationDbContext dbContext, string restCountriesUrl, IMemoryCache memoryCache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _restCountriesUrl = restCountriesUrl ?? throw new ArgumentNullException(nameof(restCountriesUrl));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        // <summary>
        /// Fetches a list of countries from the external API.
        /// </summary>
        /// <returns>A list of country DTOs.</returns>
        public async Task<List<CountryDTO>> FetchCountriesAsync()
        {
            try
            {
                List<CountryDTO> result = new List<CountryDTO>();
                result = FetchCountriesFromCache();
                if (result.Count() >0)
                    return result;

                result = FetchCountriesFromDB();
                if (result.Count > 0)
                    return result;

                var finalResult = FetchCountriesFromApiAndSaveAsync();
                return await finalResult;
            }
            catch (Exception ex) {
                throw;
            }          
        }

        // <summary>
        /// Fetches a list of countries from the external API.
        /// </summary>
        /// <returns>A list of country DTOs.</returns>
        public List<CountryDTO> FetchCountriesFromCache()
        {
            CustomMemoryCache cache = new CustomMemoryCache();
            if (cache.TryGetValue("Countries", out List<CountryDTO> cachedCountries))
            {
                // Return the cached result
                return cachedCountries;
            }
            return new List<CountryDTO>();
        }
        // <summary>
        /// Fetches a list of countries from the external API.
        /// </summary>
        /// <returns>A list of country DTOs.</returns>
        public List<CountryDTO> FetchCountriesFromDB()
        {
            if (!_dbContext.Countries.Any())
                return new List<CountryDTO>();

            try
            {
                // Fetch and map countries from DB to CountryDTO
                var countriesDto =  _dbContext.Countries
                    .Select(country => new CountryDTO
                    {
                        Name = country.Name ,
                        Borders = country.Borders.Select(b => b.CountryBorder).ToList(),
                        Capital = country.Capital.Select(c => c.CapitalCode).ToList()
                    })
                    .ToList();

                AddToCache("Countries", countriesDto);

                return countriesDto; // Return the list of CountryDTO
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during fetching data from database: {ex.Message}");
                throw new InvalidOperationException("Failed to fetch data from database.", ex);
            }
        }
        // <summary>
        /// Fetches a list of countries from the external API.
        /// </summary>
        /// <returns>A list of country DTOs.</returns>
        public async Task<List<CountryDTO>> FetchCountriesFromApiAndSaveAsync()
        {
           try
            {
                var countries = await FetchCountriesFromApiAsync();

                var countriesCustomized = countries.Select(country => new CountryDTO()
                {
                    Name = country.Name.Common, // Extract only the 'Common' field from NameDTO
                    Borders = country.Borders,
                    Capital = country.Capital
                }).ToList();

                await SaveCountriesToDatabaseAsync(countriesCustomized);
                //save to cache
                AddToCache("Countries", countriesCustomized);

                return countriesCustomized;
            }
            catch (Exception ex)
            {            
                throw;
            }
        }
        public void AddToCache (string key, List<CountryDTO> countries)
        {
            _memoryCache.Set(key, countries, TimeSpan.FromHours(1));

        }
        // <summary>
        /// Fetches a list of countries from the external API.
        /// </summary>
        /// <returns>A list of country DTOs.</returns>
        public async Task<List<CountryResponseDTO>> FetchCountriesFromApiAsync()
        {
            if (string.IsNullOrWhiteSpace(_restCountriesUrl))
                throw new ArgumentException("The RestCountries URL is not configured properly.", nameof(_restCountriesUrl));

            try
            {
                var response = await _httpClient.GetAsync(_restCountriesUrl);
                response.EnsureSuccessStatusCode(); // Throws if the status code is not successful

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CountryResponseDTO>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during HTTP GET: {ex.Message}");
                throw new InvalidOperationException("Failed to fetch data from the external API.", ex);
            }
        }

        public async Task SaveCountriesToDatabaseAsync(List<CountryDTO> countries)
        {
            // Save logic here
            if (countries != null)
            {
                var countriesToAdd = countries
                    .Select(country => new Country
                    {
                        Name = country.Name,
                        Borders = country.Borders != null
                            ? country.Borders.Select(b => new Border { CountryBorder = b }).ToList()
                            : new List<Border>(),
                        Capital = country.Capital != null
                            ? country.Capital.Select(c => new Capital { CapitalCode = c }).ToList()
                            : new List<Capital>()
                    })
                    .ToList();  // Convert the result to a list

                // Add all the countries to the DbContext in one go
                _dbContext.Countries.AddRange(countriesToAdd);

                // Save all the added countries to the database
                await _dbContext.SaveChangesAsync();
            }

        }

        // Dispose method to clean up any resources
        public void Dispose()
        {
            if (!_disposed)
            {
                _dbContext?.Dispose();
                _disposed = true;
            }
        }
    }
}
