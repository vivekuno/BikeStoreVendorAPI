using BikeStoreVendor.Data.Access;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeStoreVendor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IDapper _dapper;
        public ReportController(IDapper dapper)
        {
            _dapper = dapper;
        }

        [HttpGet("highest-selling")]
        public async Task<IActionResult> GetHighestSellingProduct([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be later than end date.");
            }
            BL.Report report = new BL.Report(_dapper);

            var highestSellingProduct = report.GetHighestSellingProductAsync(startDate, endDate);

            if (highestSellingProduct == null)
            {
                return NotFound("No sales data found for the given date range.");
            }

            return Ok(highestSellingProduct);
        }
    }
}
