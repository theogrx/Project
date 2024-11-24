using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.Controllers; // Adjust namespace
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project.Tests
{
    [TestClass]
    public class CountryControllerIntegrationTests
    {
        private readonly CountryController _controller;

        public CountryControllerIntegrationTests()
        {
            // Use the real HttpClient
            var httpClient = new HttpClient();

            // Mock ILogger (optional)
            var mockLogger = new Mock<ILogger<CountryController>>();

            // Create the controller with the real HttpClient
            _controller = new CountryController(mockLogger.Object, httpClient);
        }

        [TestMethod]
        public async Task GetCountries_ReturnsValidResponse_FromLiveApi()
        {
            // Act
            var result = await _controller.GetCountries() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            var countries = result.Value as IEnumerable<dynamic>;
            Assert.IsNotNull(countries, "Response should contain a list of countries.");

            // Verify the data structure of the response
            Assert.IsTrue(countries.Any(), "The countries list should not be empty.");

            // Check a specific country to ensure the data structure is valid
            var firstCountry = countries.First();
            Assert.IsNotNull(firstCountry.CommonName, "CommonName should not be null.");
            Assert.IsNotNull(firstCountry.Capital, "Capital should not be null.");
            Assert.IsNotNull(firstCountry.Borders, "Borders should not be null.");
        }
    }
}
