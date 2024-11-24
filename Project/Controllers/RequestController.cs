using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Models;

namespace Project.Controllers
{
    public class RequestController : ControllerBase
    {
        private readonly ILogger<RequestController> _logger;

        public RequestController(ILogger<RequestController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult GetSecondLargest(RequestObj request)
        {
            var sortedArray = request.RequestArrayObj.Distinct().OrderByDescending(x => x).ToList();
            if (sortedArray.Count > 1)
            {
                return Ok(sortedArray[1]);
            }
            return BadRequest("Array should have at least two distinct elements.");
        }
    }
}
