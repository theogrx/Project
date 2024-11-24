using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Models;

public class CountryController : Controller
{
    private readonly ILogger<CountryController> _logger;
    private readonly HttpClient _client;

    public CountryController(ILogger<CountryController> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> GetCountries()
    {
        var response = await _client.GetStringAsync("https://restcountries.com/v3.1/all");
        var countries = JsonConvert.DeserializeObject<List<Country>>(response);

        return Ok(countries.Select(c => new
        {
            CommonName = c.Name.Common,
            Capital = c.Capital,
            Borders = c.Borders
        }));
    }
}
