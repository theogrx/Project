using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Business.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Project.Controllers;
using Project.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace Project.Tests
{
    [TestClass]
    public class CountryControllerTests
    {
        private HttpClient _httpClient;
        private IOptions<ApiSettings> _apiSettings;
        private CustomMemoryCache _memoryCache;
        private ApplicationDbContext _dbContext;

        [TestInitialize]
        public void TestInitialize()
        {
            // Set up HttpClient with a real endpoint (NOTE: This makes the test dependent on an external API)
            _httpClient = new HttpClient();

            // Set up ApiSettings to include the real URL
            var apiSettings = new ApiSettings
            {
                RestCountriesUrl = "https://restcountries.com/v3.1/all"
            };
            _apiSettings = Options.Create(apiSettings);
            _memoryCache = new CustomMemoryCache();
            _dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Data Source=(local);Initial Catalog=Project;Integrated Security=True;Encrypt=False")
                    .Options);
        }

        [TestMethod]
        public async Task GetCountriesAsync_ReturnsCountries_WhenApiCallIsSuccessful()
        {
            try
            {
                // Arrange
                // Use real HttpClient with the actual endpoint
                var controller = new CountryController(_httpClient, _apiSettings, _memoryCache, _dbContext);

                // Act
                var result = await controller.GetCountriesAsync();
                var okResult = result as OkObjectResult;

                var returnValue = (List<CountryDTO>)okResult.Value; // Extract the return value
                Assert.IsTrue(returnValue.Count > 0); // Ensure the result is not empty
            }
            catch (Exception ex) { 
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod]
        public async Task GetCountriesAsync_ReturnsBadRequest_WhenApiCallFails()
        {
            // Arrange
            var mockHttpClient = new Mock<HttpClient>();
            mockHttpClient.Setup(client => client.GetAsync(It.IsAny<string>()))
                          .ThrowsAsync(new Exception("API call failed"));

            // Set up mock API settings
            var apiSettings = new ApiSettings
            {
                RestCountriesUrl = "https://restcountries.com/v3.1/all"
            };
            _apiSettings = Options.Create(apiSettings);

            // Create controller with mocked HttpClient
            var controller = new CountryController(mockHttpClient.Object, _apiSettings, _memoryCache, _dbContext);

            // Act
            var result = await controller.GetCountriesAsync();

            // Assert
            var okResult = result as OkObjectResult;

            var returnValue = (List<CountryDTO>)okResult.Value;// Extract the return value
        }
    }
}
