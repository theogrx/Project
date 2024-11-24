using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Project.Controllers;  // Adjust based on your actual namespace
using Project.Models;

namespace Project.Tests
{
    [TestClass]
    public class RequestControllerTests
    {
        private readonly RequestController _controller;

        public RequestControllerTests()
        {
            // Mocking the logger
            var mockLogger = new Mock<ILogger<RequestController>>();
            _controller = new RequestController(mockLogger.Object);
        }

        [TestMethod]
        public async Task GetSecondLargest_ReturnsSecondLargest_WhenValidArrayIsPassed()
        {
            // Arrange: Create a request with a list of integers
            var request = new RequestObj
            {
                RequestArrayObj = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 }
            };

            // Act: Call the GetSecondLargest method on the controller
            var result = _controller.GetSecondLargest(request) as OkObjectResult;

            // Assert: Verify the result is not null and contains the expected second largest value
            var response = result?.Value as dynamic;

            // Assert the second largest number is 6 (based on the example array)
            Assert.IsNotNull(response);  // Ensure a value is returned
            Assert.AreEqual(6, response); // 6 is the second largest number in the array
        }
    }
}
